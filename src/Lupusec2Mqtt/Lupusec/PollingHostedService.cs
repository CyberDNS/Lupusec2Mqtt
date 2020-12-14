using System;
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
            SensorList response = await _lupusecService.GetSensorsAsync();

            foreach (var sensor in response.Sensors)
            {
                IDevice config = _conversionService.GetDevice(sensor);
                if (config != null) { _mqttService.Publish(config.ConfigTopic, JsonConvert.SerializeObject(config)); }
            }

            PanelCondition panelCondition = await _lupusecService.GetPanelConditionAsync();
            var panelConditions = _conversionService.GetDevice(panelCondition);

            _mqttService.Register(panelConditions.Area1.CommandTopic, mode => { SetAlarm(1, mode); });
            _mqttService.Register(panelConditions.Area2.CommandTopic, mode => { SetAlarm(2, mode); });

            _mqttService.Publish(panelConditions.Area1.ConfigTopic, JsonConvert.SerializeObject(panelConditions.Area1));
            _mqttService.Publish(panelConditions.Area2.ConfigTopic, JsonConvert.SerializeObject(panelConditions.Area2));

            AlarmBinarySensor alarmBinarySensorArea1 = new AlarmBinarySensor(_configuration, 1);
            AlarmBinarySensor alarmBinarySensorArea2 = new AlarmBinarySensor(_configuration, 2);

            _mqttService.Publish(alarmBinarySensorArea1.ConfigTopic, JsonConvert.SerializeObject(alarmBinarySensorArea1));
            _mqttService.Publish(alarmBinarySensorArea2.ConfigTopic, JsonConvert.SerializeObject(alarmBinarySensorArea2));

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
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

        private async void DoWork(object state)
        {
            try
            {
                if (--_logCounter <= 0) {
                    _logger.LogInformation($"Polling... (Every {_logEveryNCycle}th cycle is logged)");
                    _logCounter = _logEveryNCycle;
                }

                SensorList sensorList = await _lupusecService.GetSensorsAsync();
                _logger.LogDebug("Received {countSensors} sensors", sensorList.Sensors.Count());

                RecordList recordList = await _lupusecService.GetRecordsAsync();
                _logger.LogDebug("Received records");

                foreach (var sensor in sensorList.Sensors)
                {
                    IStateProvider device = _conversionService.GetStateProvider(sensor, recordList.Logrows.Where(r => r.Sid.Equals(sensor.SensorId)).ToArray());
                    if (device != null) { _mqttService.Publish(device.StateTopic, device.State); }
                }

                PanelCondition panelCondition = await _lupusecService.GetPanelConditionAsync();
                var panelConditions = _conversionService.GetDevice(panelCondition);
                _logger.LogDebug("Received alarm panel information (Area 1: {area1}, Area 2: {area2})", panelConditions.Area1.State, panelConditions.Area2.State);

                _mqttService.Publish(panelConditions.Area1.StateTopic, panelConditions.Area1.State);
                _mqttService.Publish(panelConditions.Area2.StateTopic, panelConditions.Area2.State);

                AlarmBinarySensor alarmBinarySensorArea1 = new AlarmBinarySensor(_configuration, 1);
                AlarmBinarySensor alarmBinarySensorArea2 = new AlarmBinarySensor(_configuration, 2);

                alarmBinarySensorArea1.SetState(sensorList.Sensors);
                alarmBinarySensorArea2.SetState(sensorList.Sensors);

                _mqttService.Publish(alarmBinarySensorArea1.StateTopic, alarmBinarySensorArea1.State);
                _mqttService.Publish(alarmBinarySensorArea2.StateTopic, alarmBinarySensorArea2.State);
            }
            catch (HttpRequestException ex)
            {
                // Log and retry on next iteration
                _logger.LogError(ex, "An error occured");
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