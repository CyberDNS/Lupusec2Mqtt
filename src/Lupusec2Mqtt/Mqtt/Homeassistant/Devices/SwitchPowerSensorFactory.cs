﻿using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class SwitchPowerSensorFactory : DeviceFactory
    {
        public SwitchPowerSensorFactory(IConfiguration configuration, ILupusecService lupusecService) 
            : base(configuration, lupusecService)
        { }

        public override Task<IEnumerable<Device>> GenerateDevicesAsync()
        {
            var result = _lupusecService.PowerSwitchList.PowerSwitches
             .Where(s => s.Type == 48)
             .Select(s => new SwitchPowerSensor(s))
             .ToArray();

            return Task.FromResult<IEnumerable<Device>>(result);
        }
    }
}
