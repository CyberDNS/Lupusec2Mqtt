using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class HumiditySensor : Device
    {
        private readonly string _id;

        public override string Component => "sensor";

        public HumiditySensor(Sensor sensor)
        {
            _id = sensor.SensorId;

            DeclareStaticValue("name", sensor.Name + " - Humidity");
            DeclareStaticValue("unique_id", sensor.SensorId + "HUMIDITY");
            DeclareStaticValue("device_class", "humidity");
            DeclareStaticValue("unit_of_measurement", "%");

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue("unique_id")}/state", GetState);
        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            var sensor = lupusecService.SensorList.Sensors.Single(s => s.SensorId == _id);
            var match = Regex.Match(sensor.Status, @"{WEB_MSG_RH_HUMIDITY}\s*(?'value'\d+\.?\d*)");

            if (match.Success) { return Task.FromResult(match.Groups["value"].Value); }
            return Task.FromResult("0");
        }
    }
}
