using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.DevicesNew
{
    public class AlarmDetectionSensorFactory : DeviceFactory
    {
        public AlarmDetectionSensorFactory(IConfiguration configuration, ILupusecService lupusecService)
            : base(configuration, lupusecService)
        { }

        public override Task<IEnumerable<Device>> GenerateDevicesAsync()
        {
            var alarmBinarySensors = new AlarmDetectionSensor[]
            {
                new AlarmDetectionSensor(1),
                new AlarmDetectionSensor(2),
            };

            return Task.FromResult<IEnumerable<Device>>(alarmBinarySensors);
        }
    }
}
