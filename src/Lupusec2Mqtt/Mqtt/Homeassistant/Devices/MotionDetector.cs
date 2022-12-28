using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class MotionDetector : Device
    {
        private readonly IConfiguration _configuration;

        public override string Component => "binary_sensor";

        public MotionDetector(IConfiguration configuration, Sensor sensor)
        {
            _configuration = configuration;

            DeclareStaticValue("name", sensor.Name); 
            DeclareStaticValue("unique_id", sensor.SensorId);
            DeclareStaticValue("device_class", "motion");

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/state", GetState);
        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            var matchingEvent = lupusecService.RecordList.Logrows.Where(r => r.Event.StartsWith("{ALARM_HISTORY_20}") && r.Sid.Equals(GetStaticValue<string>("unique_id")))
            .OrderByDescending(r => r.UtcDateTime)
            .FirstOrDefault(r => (DateTime.UtcNow - r.UtcDateTime) <= TimeSpan.FromSeconds(_configuration.GetValue<int>("MotionSensor:DetectionDuration")));

            return Task.FromResult(matchingEvent != null ? "ON" : "OFF");
        }
    }
}
