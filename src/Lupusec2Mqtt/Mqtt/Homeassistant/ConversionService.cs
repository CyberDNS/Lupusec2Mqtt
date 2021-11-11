using System;
using System.Collections;
using System.Collections.Generic;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt.Homeassistant.Devices;
using Microsoft.Extensions.Configuration;

namespace Lupusec2Mqtt.Mqtt.Homeassistant
{
    public class ConversionService
    {
        private readonly IConfiguration _configuration;

        public ConversionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<IDevice> GetDevices(Sensor sensor)
        {
            List<IDevice> list = new List<IDevice>();
            switch (sensor.TypeId)
            {
                case 4: // Opener contact
                case 9: // Motion detector
                case 11: // Smoke detector
                case 5: // Water detector
                    list.Add(new BinarySensor(_configuration, sensor));
                    return list;
                case 54: // Temp/Humidity detector
                    list.Add(new TemperatureSensor(_configuration, sensor));
                    list.Add(new HumiditySensor(_configuration, sensor));
                    return list;
                default:
                    return null;
            }
        }

        public (ISettable Device, SwitchPowerSensor SwitchPowerSensor)? GetDevice(PowerSwitch powerSwitch)
        {
            switch (powerSwitch.Type)
            {
                case 48: // Power meter switch
                    return (Device: new Switch(_configuration, powerSwitch), SwitchPowerSensor: new SwitchPowerSensor(_configuration, powerSwitch));
                case 74: // Light switch
                    return (Device: new Light(_configuration, powerSwitch), SwitchPowerSensor: null);
                case 57: // Smart Lock
                    return (Device: new Lock(_configuration, powerSwitch), SwitchPowerSensor: null);
                default:
                    return null;
            }
        }

        public (AlarmControlPanel Area1, AlarmControlPanel Area2) GetDevice(PanelCondition panelCondition)
        {
            return (Area1: new AlarmControlPanel(_configuration, panelCondition, 1), Area2: new AlarmControlPanel(_configuration, panelCondition, 2));
        }

        public IEnumerable<IStateProvider> GetStateProviders(Sensor sensor, IEnumerable<Logrow> logRows)
        {
            List<IStateProvider> list = new List<IStateProvider>();
            switch (sensor.TypeId)
            {
                case 4: // Opener contact
                case 9: // Motion detector
                case 11: // Smoke detector
                case 5: // Water detector
                    list.Add(new BinarySensor(_configuration, sensor, logRows));
                    return list;
                case 54: // Temp/Humidity detector
                    list.Add(new TemperatureSensor(_configuration, sensor, logRows));
                    list.Add(new HumiditySensor(_configuration, sensor, logRows));
                    return list;
                default:
                    return null;
            }
        }

        public (IStateProvider Device, IStateProvider SwitchPowerSensor)? GetStateProvider(PowerSwitch powerSwitch)
        {
            switch (powerSwitch.Type)
            {
                case 48: // Power meter switch
                    return (Device: new Switch(_configuration, powerSwitch), SwitchPowerSensor: new SwitchPowerSensor(_configuration, powerSwitch));
                case 74: // Light switch
                    return (Device: new Light(_configuration, powerSwitch), SwitchPowerSensor: null);
                case 57: // Smart Lock
                    return (Device: new Lock(_configuration, powerSwitch), SwitchPowerSensor: null);
                default:
                    return null;
            }

        }
    }
}
