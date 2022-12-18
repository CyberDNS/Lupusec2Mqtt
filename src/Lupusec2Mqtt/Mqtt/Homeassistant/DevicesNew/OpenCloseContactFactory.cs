using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.DevicesNew
{
    public class OpenCloseContactFactory : DeviceFactory
    {
        public OpenCloseContactFactory(IConfiguration configuration, ILupusecService lupusecService)
            : base(configuration, lupusecService)
        { }

        public override Task<IEnumerable<Device>> GenerateDevicesAsync()
        {
            var result = _lupusecService.SensorList.Sensors
                .Where(s => s.TypeId == 4 || s.TypeId == 33)
                .Select(s => new OpenCloseContact(s))
                .ToArray();

            return Task.FromResult<IEnumerable<Device>>(result);
        }
    }
}
