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
    public class SwitchPowerSensor : Device, IDevice, IStateProvider
    {
        protected readonly PowerSwitch _powerSwitch;

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonProperty("unit_of_measurement")]
        public string UnitOfMeasurement => "W";

        [JsonIgnore]
        public string State => GetState();

        protected override string _component => "sensor";


        private string GetState()
        {
            var match = Regex.Match(_powerSwitch.Status, @"{WEB_MSG_PSM_POWER}\s*(?'power'\d+\.?\d*)");

            if (match.Success) { return match.Groups["power"].Value; }
            return "0";
        }

        public SwitchPowerSensor(IConfiguration configuration, PowerSwitch powerSwitch)
       : base(configuration)
        {
            _powerSwitch = powerSwitch;

            UniqueId = $"{_powerSwitch.Id}_power";
            Name = GetValue(nameof(Name), $"{_powerSwitch?.Name??_powerSwitch.Id} - Power");
        }
    }
}
