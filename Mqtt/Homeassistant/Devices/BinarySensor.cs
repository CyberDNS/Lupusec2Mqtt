using System;
using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class BinarySensor : IDevice, IStateProvider
    {
        private readonly Sensor _sensor;
        private readonly IConfiguration _configuration;

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("unique_id")]
        public string UniqueId { get; set; }

        [JsonProperty("device_class")]
        public string DeviceClass { get; set; }

        [JsonIgnore]
        public string ConfigTopic => $"homeassistant/binary_sensor/lupusec/{_sensor.SensorId.Replace(":", "")}/config";

        [JsonProperty("state_topic")]
        public string StateTopic => $"homeassistant/binary_sensor/lupusec/{_sensor.SensorId.Replace(":", "")}/state";

        [JsonIgnore]
        public string State => GetState();

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
        {
            _configuration = configuration;
            _sensor = sensor;

            UniqueId = _sensor.SensorId.Replace(":", "");
            Name = GetValue(nameof(Name), sensor.Name);
            DeviceClass = GetValue(nameof(DeviceClass), GetDeviceClassDefaultValue());
        }

        private string GetValue(string property, string defaultValue)
        {
            return _configuration[$"Mappings:{_sensor.SensorId}:{property}"] ?? defaultValue;
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