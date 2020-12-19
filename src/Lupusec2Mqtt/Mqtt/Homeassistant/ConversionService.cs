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

        public IDevice GetDevice(Sensor sensor)
        {
            switch (sensor.TypeId)
            {
                case 4: // Opener contact
                case 9: // Motion detector
                case 11: // Smoke detector
                case 5: // Water detector
                    return new BinarySensor(_configuration, sensor);
                case 48: // Power meter switch
                    return null;
                default:
                    return null;
            }
        }

        public (Switch Switch, SwitchPowerSensor SwitchPowerSensor) GetDevice(PowerSwitch powerSwitch)
        {
            return (Switch: new Switch(_configuration, powerSwitch), SwitchPowerSensor: new SwitchPowerSensor(_configuration, powerSwitch));
        }

        public (AlarmControlPanel Area1, AlarmControlPanel Area2) GetDevice(PanelCondition panelCondition)
        {
            return (Area1: new AlarmControlPanel(_configuration, panelCondition, 1), Area2: new AlarmControlPanel(_configuration, panelCondition, 2));
        }

        public IStateProvider GetStateProvider(Sensor sensor, IEnumerable<Logrow> logRows)
        {
            switch (sensor.TypeId)
            {
                case 4: // Opener contact
                case 9: // Motion detector
                case 11: // Smoke detector
                case 5: // Water detector
                    return new BinarySensor(_configuration, sensor, logRows);
                case 48: // Power meter switch
                    return null;
                default:
                    return null;
            }
        }

        public (IStateProvider Switch, IStateProvider SwitchPowerSensor) GetStateProvider(PowerSwitch powerSwitch)
        {
            return (Switch: new Switch(_configuration, powerSwitch), SwitchPowerSensor: new SwitchPowerSensor(_configuration, powerSwitch));
        }
    }
}
