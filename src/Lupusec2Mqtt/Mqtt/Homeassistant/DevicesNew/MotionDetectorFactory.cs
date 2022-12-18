using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.DevicesNew
{
    public class MotionDetectorFactory : DeviceFactory
    {
        public MotionDetectorFactory(IConfiguration configuration, ILupusecService lupusecService)
            : base(configuration, lupusecService)
        { }

        public override Task<IEnumerable<Device>> GenerateDevicesAsync()
        {
            var result = _lupusecService.SensorList.Sensors
                .Where(s => s.TypeId == 9)
                .Select(s => new MotionDetector(_configuration, s))
                .ToArray();

            return Task.FromResult<IEnumerable<Device>>(result);
        }
    }
}
