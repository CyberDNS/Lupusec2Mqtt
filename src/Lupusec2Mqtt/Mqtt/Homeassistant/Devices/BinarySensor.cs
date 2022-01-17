using System;
using System.Collections.Generic;
using System.Linq;
using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class BinarySensor : Device, IDevice, IStateProvider
    {
        protected readonly Sensor _sensor;
        protected readonly IList<Logrow> _logRows;

        [JsonProperty("device_class")]
        public string DeviceClass { get; set; }

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonIgnore]
        public string State => GetState();

        protected override string _component => "binary_sensor";

        private string GetState()
        {
            switch (_sensor.TypeId)
            {
                case 4: // Opener contact
                    return _sensor.Status == "{WEB_MSG_DC_OPEN}" ? "ON" : "OFF";
                case 9: // Motion detector
                    var matchingEvent = _logRows.Where(r => r.Event.StartsWith("{ALARM_HISTORY_20}"))
                        .OrderByDescending(r => r.UtcDateTime)
                        .FirstOrDefault(r => (DateTime.UtcNow - r.UtcDateTime) <= TimeSpan.FromSeconds(_configuration.GetValue<int>("MotionSensor:DetectionDuration")));

                    return matchingEvent != null ? "ON" : "OFF";
                case 11: // Smoke detector
                    return _sensor.Status == "{RPT_CID_111}" ? "ON" : "OFF";
                case 5: // Water detector
                    return "OFF";
                default:
                    return null;
            }
        }

        public BinarySensor(IConfiguration configuration, Sensor sensor, IList<Logrow> logRows = default)
        : base(configuration)
        {
            _sensor = sensor;
            _logRows = logRows??new Logrow[0];

            UniqueId = _sensor.SensorId;
            Name = GetValue(nameof(Name), sensor.Name);
            DeviceClass = GetValue(nameof(DeviceClass), GetDeviceClassDefaultValue());
        }

        private string GetDeviceClassDefaultValue()
        {
            switch (_sensor.TypeId)
            {
                case 4:
                    return "window";
                case 9:
                    return "motion";
                case 11:
                    return "smoke";
                case 5:
                    return "moisture";
                default:
                    return null;
            }
        }
    }
}