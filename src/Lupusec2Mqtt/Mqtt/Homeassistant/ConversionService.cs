using System;
using System.Collections;
using System.Collections.Generic;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt.Homeassistant.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lupusec2Mqtt.Mqtt.Homeassistant
{
    public class ConversionService
    {
        private readonly DeviceAnalyzer _deviceAnalyzer;
        private readonly DeviceListAnalyzer _deviceListAnalyzer;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public ConversionService(IConfiguration configuration,ILogger logger)
        {
            _deviceAnalyzer=new DeviceAnalyzer(configuration,logger);
            _deviceListAnalyzer=new DeviceListAnalyzer(configuration,logger);
            _configuration = configuration;
            _logger = logger;
        }

        public IEnumerable<IDevice> GetDevices(Sensor sensor)
        {
             return _deviceListAnalyzer.Analyze(sensor);
        }

        public (ISettable Device, IDevice SwitchPowerSensor)? GetDevice(PowerSwitch powerSwitch)
        {
            return _deviceAnalyzer.Analyze(powerSwitch);
        }

        public (AlarmControlPanel Area1, AlarmControlPanel Area2) GetDevice(PanelCondition panelCondition)
        {
            return (Area1: new AlarmControlPanel(_configuration, panelCondition, 1), Area2: new AlarmControlPanel(_configuration, panelCondition, 2));
        }

        public IEnumerable<IStateProvider> GetStateProviders(Sensor sensor, IEnumerable<Logrow> logRows)
        {
             return _deviceListAnalyzer.Analyze(sensor);
        }

        public (IStateProvider Device, IStateProvider SwitchPowerSensor)? GetStateProvider(PowerSwitch powerSwitch)
        {
             return _deviceAnalyzer.Analyze(powerSwitch);
        }
    }
}
