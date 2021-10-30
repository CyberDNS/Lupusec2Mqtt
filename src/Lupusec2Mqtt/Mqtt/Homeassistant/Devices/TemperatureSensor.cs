using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class TemperatureSensor : Device, IDevice, IStateProvider
    {
        protected readonly Sensor _sensor;
        protected readonly IEnumerable<Logrow> _logRows;

        [JsonProperty("device_class")]
        public string DeviceClass { get; set; }

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonIgnore]
        public string State => GetState();

        [JsonProperty("unit_of_measurement")]
        public string UnitOfMeasurement => "\x00B0C";

        protected override string _component => "sensor";

        private string GetState()
        {
            var match = Regex.Match(_sensor.Status, @"{WEB_MSG_TS_DEGREE}\s*(?'value'\d+\.?\d*)");

            if (match.Success) { return match.Groups["value"].Value; }
            return "0";
        }

        public TemperatureSensor(IConfiguration configuration, Sensor sensor, IEnumerable<Logrow> logRows = default)
        : base(configuration)
        {
            _sensor = sensor;
            _logRows = logRows;

            UniqueId = _sensor.SensorId + "TEMPERATURE";
            Name = GetValue(nameof(Name), sensor.Name + " - Temperature");
            DeviceClass = GetValue(nameof(DeviceClass), GetDeviceClassDefaultValue());
        }

        private string GetDeviceClassDefaultValue()
        {
            return "temperature";
        }
    }
}