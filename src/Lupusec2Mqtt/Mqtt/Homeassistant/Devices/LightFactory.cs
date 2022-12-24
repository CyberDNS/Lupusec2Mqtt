using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class LightFactory : DeviceFactory
    {
        public LightFactory(IConfiguration configuration, ILupusecService lupusecService) 
            : base(configuration, lupusecService)
        { }

        public override Task<IEnumerable<Device>> GenerateDevicesAsync()
        {
            var result = _lupusecService.PowerSwitchList.PowerSwitches
                .Where(s => s.Type == 74)
                .Select(s => new Light(s))
                .ToArray();

            return Task.FromResult<IEnumerable<Device>>(result);
        }
    }
}
