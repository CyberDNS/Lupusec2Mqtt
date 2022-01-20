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
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public ConversionService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IList<IDevice> GetDevices(Sensor sensor)
        {
            List<IDevice> list = new List<IDevice>();

            switch (sensor.TypeId)
            {
                case 4: // Opener contact
                case 33: // Opener contact XT2
                case 9: // Motion detector
                case 11: // Smoke detector
                case 5: // Water detector
                    list.Add(new BinarySensor(_configuration, sensor));
                    break;
                case 54: // Temperature/Humidity detector
                    list.Add(new TemperatureSensor(_configuration, sensor));
                    list.Add(new HumiditySensor(_configuration, sensor));
                    break;
                default:
                    LogIgnoredSensor(sensor);
                    break;
            }

            return list;
        }

        public (ISettable Device, SwitchPowerSensor SwitchPowerSensor)? GetDevice(PowerSwitch powerSwitch)
        {
            switch (powerSwitch.Type)
            {
                case 24: // Wall switch
                    return (Device: new Switch(_configuration, powerSwitch), SwitchPowerSensor: null);
                case 48: // Power meter switch
                    return (Device: new Switch(_configuration, powerSwitch), SwitchPowerSensor: new SwitchPowerSensor(_configuration, powerSwitch));
                case 74: // Light switch
                    return (Device: new Light(_configuration, powerSwitch), SwitchPowerSensor: null);
                case 57: // Smart Lock
                    return (Device: new Lock(_configuration, powerSwitch), SwitchPowerSensor: null);
                default:
                    LogIgnoredDevice(powerSwitch);
                    return null;
            }
        }

        public (AlarmControlPanel Area1, AlarmControlPanel Area2) GetDevice(PanelCondition panelCondition)
        {
            return (Area1: new AlarmControlPanel(_configuration, panelCondition, 1), Area2: new AlarmControlPanel(_configuration, panelCondition, 2));
        }

        public IList<IStateProvider> GetStateProviders(Sensor sensor, IList<Logrow> logRows)
        {
            List<IStateProvider> list = new List<IStateProvider>();

            switch (sensor.TypeId)
            {
                case 4: // Opener contact
                case 33: // Opener contact XT2
                case 9: // Motion detector
                case 11: // Smoke detector
                case 5: // Water detector
                    list.Add(new BinarySensor(_configuration, sensor, logRows));
                    break;
                case 54: // Temperature/Humidity detector
                    list.Add(new TemperatureSensor(_configuration, sensor, logRows));
                    list.Add(new HumiditySensor(_configuration, sensor, logRows));
                    break;
                default:
                    LogIgnoredSensor(sensor);
                    break;
            }

            return list;
        }

        public (IStateProvider Device, IStateProvider SwitchPowerSensor)? GetStateProvider(PowerSwitch powerSwitch)
        {
            switch (powerSwitch.Type)
            {
                case 24: // Wall switch
                    return (Device: new Switch(_configuration, powerSwitch), SwitchPowerSensor: null);
                case 48: // Power meter switch
                    return (Device: new Switch(_configuration, powerSwitch),
                            SwitchPowerSensor: new SwitchPowerSensor(_configuration, powerSwitch));
                case 74: // Light switch
                    return (Device: new Light(_configuration, powerSwitch), SwitchPowerSensor: null);
                case 57: // Smart Lock
                    return (Device: new Lock(_configuration, powerSwitch), SwitchPowerSensor: null);
                default:
                    LogIgnoredDevice(powerSwitch);
                    return null;
            }

        }

        private void LogIgnoredDevice(PowerSwitch powerSwitch)
        {
            _logger.LogDebug("Device of Type {type} with name {name} is ignored.",
                               powerSwitch.Type,
                               powerSwitch.Name);
        }

        private void LogIgnoredSensor(Sensor sensor)
        {
            _logger.LogDebug("Sensor of Type {type} with name {name} is ignored.",
                                sensor.TypeId,
                                sensor.Name);
        }
    }
}
