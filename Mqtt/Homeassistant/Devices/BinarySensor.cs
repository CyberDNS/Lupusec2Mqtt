using System;
using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class BinarySensor : Device, IDevice, IStateProvider
    {
        protected readonly Sensor _sensor;

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
                    return "Off";
                case 11: // Smoke detector
                    return _sensor.Status == "{RPT_CID_111}" ? "ON" : "OFF";
                case 5: // Water detector
                    return "Off";
                default:
                    return null;
            }
        }

        public BinarySensor(IConfiguration configuration, Sensor sensor)
        : base(configuration)
        {
            _sensor = sensor;

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