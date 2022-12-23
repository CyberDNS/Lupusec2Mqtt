using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class SwitchEnergySensor : Device, IDevice, IStateProvider
    {
        protected readonly PowerSwitch _powerSwitch;

        [JsonProperty("device_class")]
        public string DeviceClass { get; set; }

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonProperty("unit_of_measurement")]
        public string UnitOfMeasurement => "kWh";

        [JsonProperty("state_class")]
        public string StateClass => "total";

        [JsonIgnore]
        public string State => GetState();

        protected override string _component => "sensor";


        private string GetState()
        {
            var match = Regex.Match(_powerSwitch.Status, @"{WEB_MSG_POWER_METER_ENERGY}\s*(?'energy'\d+\.?\d*)");

            if (match.Success) { return match.Groups["energy"].Value; }
            return "0";
        }
   
        public SwitchEnergySensor(IConfiguration configuration, PowerSwitch powerSwitch)
       : base(configuration)
        {
            _powerSwitch = powerSwitch;

            UniqueId = $"{_powerSwitch.Id}_energy";
            Name = GetValue(nameof(Name), $"{_powerSwitch.Name} - Energy");
            DeviceClass = GetValue(nameof(DeviceClass), GetDeviceClassDefaultValue());
        }

        private static string GetDeviceClassDefaultValue()
        {
            return "energy";
        }
    }
}
