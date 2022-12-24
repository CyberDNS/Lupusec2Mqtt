using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class SmokeDetector : Device
    {
        public override string Component => "binary_sensor";

        public SmokeDetector(Sensor sensor)
        {
            DeclareStaticValue("name", sensor.Name);
            DeclareStaticValue("unique_id", sensor.SensorId);
            DeclareStaticValue("device_class", "smoke");

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue("unique_id")}/state", GetState);
        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            var sensor = lupusecService.SensorList.Sensors.Single(s => s.SensorId == GetStaticValue("unique_id"));
            var result = sensor.Status == "{RPT_CID_111}" ? "ON" : "OFF"; 

            return Task.FromResult(result);
        }
    }
}
