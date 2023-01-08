using Lupusec2Mqtt.Lupusec;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using System.Collections.Generic;
using System.Text.Json;
using System.Dynamic;
using Lupusec2Mqtt.Mqtt;
using Newtonsoft.Json.Linq;

namespace Lupusec2Mqtt
{
    public class MainLoop : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ILupusecService _lupusecService;
        private readonly IEnumerable<IDeviceFactory> _factories;

        private readonly IMqttService _mqttService;
        private Timer _timer;

        private TimeSpan _pollFrequency = TimeSpan.FromSeconds(2);

        private Dictionary<string, string> _values = new Dictionary<string, string>();

        public MainLoop(ILogger<MainLoop> logger, IConfiguration configuration, ILupusecService lupusecService, IEnumerable<IDeviceFactory> factories)
        {
            _logger = logger;
            _lupusecService = lupusecService;
            _factories = factories;
            _mqttService = new MqttService(configuration);
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            await _mqttService.StartAsync();

            List<Device> devices = await GetDevices();
            await ConfigureDevicesAsync(devices);

            _logger.LogInformation("Starting to poll every {PollDelay} seconds", _pollFrequency.TotalSeconds);
            _timer = new Timer(DoWork, null, TimeSpan.Zero, _pollFrequency);
        }

        private async Task ConfigureDevicesAsync(List<Device> devices)
        {
            foreach (var device in devices)
            {
                ExpandoObject dto = new ExpandoObject();
                foreach (StaticValue staticValue in device.StaticValues) { dto.TryAdd(staticValue.Name, staticValue.Value); }
                foreach (Query query in device.Queries) { dto.TryAdd(query.Name, query.ValueTopic); }
                foreach (Command command in device.Commands)
                {
                    dto.TryAdd(command.Name, command.CommandTopic);
                    _mqttService.Register(command.CommandTopic, async input =>
                    {
                        try
                        {
                            await command.ExecuteCommand.Invoke(_logger, _lupusecService, input);
                            _logger.LogInformation("Command {Topic} of device {Device} executed with input {Input}", command.CommandTopic, device, input);
                            await _lupusecService.PollAllAsync();
                            await UpdateStates(device);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error during command execution on command {Topic} of device {Device}!", command.CommandTopic, device);
                        }
                    });

                    _logger.LogInformation("Command {Topic} registered for device {Device}", command.CommandTopic, device);
                }

                await _mqttService.PublishAsync(device.ConfigTopic, JsonSerializer.Serialize(dto));

                _logger.LogInformation("Device configured: {Device}", device);
            }
        }

        private async void DoWork(object state)
        {
            try
            {
                List<Device> devices = await GetDevices();

                foreach (var device in devices)
                {
                    await UpdateStates(device);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during poll execution!");
            }
        }

        private async Task UpdateStates(Device device)
        {
            foreach (var query in device.Queries)
            {

                if (!_values.ContainsKey(query.ValueTopic)) { _values.Add(query.ValueTopic, null); }

                var value = await query.GetValue.Invoke(_logger, _lupusecService);
                _logger.LogTrace("Querying values for {Device} on topic {Topic} => {Value}", device, query.ValueTopic, value);

                if (_values[query.ValueTopic] != value)
                {
                    var oldValue = _values[query.ValueTopic];
                    _values[query.ValueTopic] = value;
                    await _mqttService.PublishAsync(query.ValueTopic, value);

                    _logger.LogInformation("Value for topic {Topic} on device {Device} changed from {oldValue} to {newValue}", query.ValueTopic, device, oldValue, value);
                }
            }
        }

        private async Task<List<Device>> GetDevices()
        {
            await _lupusecService.PollAllAsync();

            List<Device> devices = new List<Device>();
            foreach (var factory in _factories)
            {
                devices.AddRange(await factory.GenerateDevicesAsync());
            }

            return devices;
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            await _mqttService.StopAsync();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
