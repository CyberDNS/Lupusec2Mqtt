using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class AlarmBinarySensor : Device, IStateProvider
    {
        protected readonly int _area;

        [JsonProperty("device_class")]
        public string DeviceClass { get; set; }

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonIgnore]
        public string State { get; private set; }

        protected override string _component => "binary_sensor";

        public void SetState(IEnumerable<Sensor> sensors)
        {
            State = "OFF";

            if (sensors.Any(s => (s.Area == _area) && (s.AlarmStatus.Equals("BURGLAR", StringComparison.OrdinalIgnoreCase))))
            {
                State = "ON";
            }
        }

        public AlarmBinarySensor(IConfiguration configuration, int area)
        : base(configuration)
        {
            _area = area;

            UniqueId = $"lupusec_alarm_area{area}_alarm_status"; ;
            Name = $"Area {area} Alarm Status";
            DeviceClass = "safety";
        }
    }
}
