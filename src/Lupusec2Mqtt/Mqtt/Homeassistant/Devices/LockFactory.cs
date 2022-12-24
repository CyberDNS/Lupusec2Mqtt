using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class LockFactory : DeviceFactory
    {
        public LockFactory(IConfiguration configuration, ILupusecService lupusecService) 
            : base(configuration, lupusecService)
        { }

        public override Task<IEnumerable<Device>> GenerateDevicesAsync()
        {
            var result = _lupusecService.PowerSwitchList.PowerSwitches
                .Where(s => s.Type == 57)
                .Select(s => new Lock(s))
                .ToArray();

            return Task.FromResult<IEnumerable<Device>>(result);
        }
    }
}
