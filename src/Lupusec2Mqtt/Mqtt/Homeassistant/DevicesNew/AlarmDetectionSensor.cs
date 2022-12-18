using Lupusec2Mqtt.Mqtt.Homeassistant.Model;
using Lupusec2Mqtt.Lupusec.Dtos;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Lupusec2Mqtt.Lupusec;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.DevicesNew
{
    public class AlarmDetectionSensor : Device
    {
        private readonly int _area;

        public override string Component => "binary_sensor";

        public AlarmDetectionSensor(int area)
        {
            _area = area;

            DeclareStaticValue("name", $"Area {area} Alarm Status");
            DeclareStaticValue("unique_id", $"lupusec_alarm_area{area}_alarm_status");
            DeclareStaticValue("device_class", "safety");

            DeclareQuery("state_topic", $"homeassistant/{Component}/lupusec/{GetStaticValue("unique_id")}/state", GetState);
        }

        public Task<string> GetState(ILogger logger, ILupusecService lupusecService)
        {
            if (lupusecService.SensorList.Sensors.Any(s => (s.Area == _area) && (s.AlarmStatus.Equals("BURGLAR", StringComparison.OrdinalIgnoreCase))))
            {
                return Task.FromResult("ON");
            }
            else
            {
                return Task.FromResult("OFF");
            }
        }
    }
}
