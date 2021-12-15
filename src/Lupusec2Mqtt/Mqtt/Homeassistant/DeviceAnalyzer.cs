using System;
using System.Collections;
using System.Collections.Generic;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt.Homeassistant.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lupusec2Mqtt.Mqtt.Homeassistant
{
    class DeviceAnalyzer{
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public DeviceAnalyzer(IConfiguration configuration,ILogger logger)
        {
            _logger=logger;
            this._configuration = configuration;
        }
        public (ISettable Device, IStateProvider SwitchPowerSensor)? Analyze(PowerSwitch powerSwitch)
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
                    _logger.LogWarning("PowerSwitch of Type {typeId} is not supported. All Information about this unsupported PowerSwitch:\n{switch}",powerSwitch.Type,
                    powerSwitch);
                    return null;
            }
        }
    }
}
