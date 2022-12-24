using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class HumiditySensorFactory : DeviceFactory
    {
        public HumiditySensorFactory(IConfiguration configuration, ILupusecService lupusecService) 
            : base(configuration, lupusecService)
        { }

        public override Task<IEnumerable<Device>> GenerateDevicesAsync()
        {
            var result = _lupusecService.SensorList.Sensors
                .Where(s => s.TypeId == 54)
                .Select(s => new HumiditySensor(s))
                .ToArray();

            return Task.FromResult<IEnumerable<Device>>(result);
        }
    }
}
