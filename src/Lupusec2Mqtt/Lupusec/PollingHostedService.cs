using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt;
using Lupusec2Mqtt.Mqtt.Homeassistant;
using Lupusec2Mqtt.Mqtt.Homeassistant.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;

namespace Lupusec2Mqtt.Lupusec
{
    public class PollingHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<PollingHostedService> _logger;
        private readonly ILupusecService _lupusecService;
        private readonly ConversionService _conversionService;
        private readonly IConfiguration _configuration;

        private readonly MqttService _mqttService;
        private Timer _timer;

        private int _logCounter = 0;
        private int _logEveryNCycle = 5;

        private CancellationTokenSource _cancellationTokenSource;

        public PollingHostedService(ILogger<PollingHostedService> logger, ILupusecService lupusecService, IConfiguration configuration)
        {
            _logger = logger;
            _lupusecService = lupusecService;
            _configuration = configuration;

            _conversionService = new ConversionService(_configuration);
            _mqttService = new MqttService(_configuration);

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            await ConfigureSensors();
            await ConfigurePowerSwitches();
            await ConfigureAlarmPanels();

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
        }

        private async Task ConfigureSensors()
        {
            SensorList response = await _lupusecService.GetSensorsAsync();

            foreach (var sensor in response.Sensors)
            {
                IEnumerable<IDevice> configs = _conversionService.GetDevices(sensor);
                foreach (var config in configs)
                {
                    _mqttService.Publish(config.ConfigTopic, JsonConvert.SerializeObject(config));
                }
            }

            AlarmBinarySensor alarmBinarySensorArea1 = new AlarmBinarySensor(_configuration, 1);
            AlarmBinarySensor alarmBinarySensorArea2 = new AlarmBinarySensor(_configuration, 2);

            _mqttService.Publish(alarmBinarySensorArea1.ConfigTopic, JsonConvert.SerializeObject(alarmBinarySensorArea1));
            _mqttService.Publish(alarmBinarySensorArea2.ConfigTopic, JsonConvert.SerializeObject(alarmBinarySensorArea2));
        }

        private async Task ConfigurePowerSwitches()
        {
            PowerSwitchList response = await _lupusecService.GetPowerSwitches();

            foreach (var powerSwitch in response.PowerSwitches)
            {
                var config = _conversionService.GetDevice(powerSwitch);

                if (config.HasValue)
                {
                    _mqttService.Register(config.Value.Device.CommandTopic, state =>
                    {
                        try
                        {
                            _logger.LogInformation("Switch {UniqueId} {Name} set to {Status}", config.Value.Device.UniqueId, config.Value.Device.Name, state);
                            config.Value.Device.SetState(state, _lupusecService);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occured while setting switch {UniqueId} {Name} to {Status}", config.Value.Device.UniqueId, config.Value.Device.Name, state);
                        }
                    }
                    );

                    _mqttService.Publish(config.Value.Device.ConfigTopic, JsonConvert.SerializeObject(config.Value.Device));
                    if (config.Value.SwitchPowerSensor != null) { _mqttService.Publish(config.Value.SwitchPowerSensor.ConfigTopic, JsonConvert.SerializeObject(config.Value.SwitchPowerSensor)); }
                }
                else
                {
                    _logger.LogInformation("Unknown power switch type {Type} detected", powerSwitch.Type);
                }
            }
        }


        private async Task ConfigureAlarmPanels()
        {
            PanelCondition panelCondition = await _lupusecService.GetPanelConditionAsync();
            var panelConditions = _conversionService.GetDevice(panelCondition);

            _mqttService.Register(panelConditions.Area1.CommandTopic, mode => { SetAlarm(1, mode); });
            _mqttService.Register(panelConditions.Area2.CommandTopic, mode => { SetAlarm(2, mode); });

            _mqttService.Publish(panelConditions.Area1.ConfigTopic, JsonConvert.SerializeObject(panelConditions.Area1));
            _mqttService.Publish(panelConditions.Area2.ConfigTopic, JsonConvert.SerializeObject(panelConditions.Area2));
        }



        private async void DoWork(object state)
        {
            try
            {
                if (--_logCounter <= 0)
                {
                    _logger.LogInformation($"Polling... (Every {_logEveryNCycle}th cycle is logged)");
                    _logCounter = _logEveryNCycle;
                }

                await PublishSensors();
                await PublishPowerSwitches();
                await PublishAlarmPanels();
            }
            catch (HttpRequestException ex)
            {
                // Log and retry on next iteration
                _logger.LogError(ex, "An error occured");
            }
        }


        private async Task PublishPowerSwitches()
        {
            PowerSwitchList powerSwitchList = await _lupusecService.GetPowerSwitches();
            _logger.LogDebug("Received {countPowerSwitches} power switches", powerSwitchList.PowerSwitches.Count());

            foreach (var powerSwitch in powerSwitchList.PowerSwitches)
            {
                var device = _conversionService.GetStateProvider(powerSwitch);

                if (device.HasValue)
                {
                    _mqttService.Publish(device.Value.Device.StateTopic, device.Value.Device.State);
                    if (device.Value.SwitchPowerSensor != null) { _mqttService.Publish(device.Value.SwitchPowerSensor.StateTopic, device.Value.SwitchPowerSensor.State); }
                }
            }
        }

        private async Task PublishSensors()
        {
            SensorList sensorList = await _lupusecService.GetSensorsAsync();
            _logger.LogDebug("Received {countSensors} sensors", sensorList.Sensors.Count());

            RecordList recordList = await _lupusecService.GetRecordsAsync();
            _logger.LogDebug("Received records");

            foreach (var sensor in sensorList.Sensors)
            {
                _logger.LogDebug("Handling sensor of type {sensorType}", sensor.TypeId);
                IEnumerable<IStateProvider> devices = _conversionService.GetStateProviders(sensor, recordList.Logrows.Where(r => r.Sid.Equals(sensor.SensorId)).ToArray());

                _logger.LogDebug("Received {countDevices} devices", devices.Count());
                foreach (var device in devices)
                {
                    _logger.LogDebug("Publish {deviceName} device", device.Name);
                    _mqttService.Publish(device.StateTopic, device.State);
                }
            }

            AlarmBinarySensor alarmBinarySensorArea1 = new AlarmBinarySensor(_configuration, 1);
            AlarmBinarySensor alarmBinarySensorArea2 = new AlarmBinarySensor(_configuration, 2);

            alarmBinarySensorArea1.SetState(sensorList.Sensors);
            alarmBinarySensorArea2.SetState(sensorList.Sensors);

            _mqttService.Publish(alarmBinarySensorArea1.StateTopic, alarmBinarySensorArea1.State);
            _mqttService.Publish(alarmBinarySensorArea2.StateTopic, alarmBinarySensorArea2.State);
        }

        private async Task PublishAlarmPanels()
        {
            PanelCondition panelCondition = await _lupusecService.GetPanelConditionAsync();
            var panelConditions = _conversionService.GetDevice(panelCondition);
            _logger.LogDebug("Received alarm panel information (Area 1: {area1}, Area 2: {area2})", panelConditions.Area1.State, panelConditions.Area2.State);

            _mqttService.Publish(panelConditions.Area1.StateTopic, panelConditions.Area1.State);
            _mqttService.Publish(panelConditions.Area2.StateTopic, panelConditions.Area2.State);
        }

        private void SetAlarm(int area, string mode)
        {
            try
            {
                _logger.LogInformation("Area {Area} set to {Mode}", area, mode);
                _lupusecService.SetAlarmMode(area, (AlarmMode)Enum.Parse(typeof(AlarmModeAction), mode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while setting alarm mode to Area {Area} set to {Mode}", area, mode);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            _mqttService.Disconnect();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}