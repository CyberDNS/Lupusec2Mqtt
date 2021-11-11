using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class Switch : Device, IDevice, IStateProvider, ISettable
    {
        protected readonly PowerSwitch _powerSwitch;

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonProperty("command_topic")]
        public string CommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set");

        protected override string _component => "switch";

        [JsonIgnore]
        public string State => GetState();

        private string GetState()
        {
            if (_powerSwitch.Status.Contains("{WEB_MSG_PSS_ON}")) { return "ON"; } 
            else { return "OFF"; }
        }

        public void SetState(string state, ILupusecService lupusecService)
        {
            lupusecService.SetSwitch(UniqueId, state.Equals("on", StringComparison.OrdinalIgnoreCase));
        }

        public Switch(IConfiguration configuration, PowerSwitch powerSwitch)
        : base(configuration)
        {
            _powerSwitch = powerSwitch;

            UniqueId = _powerSwitch.Id;
            Name = GetValue(nameof(Name), _powerSwitch.Name);
        }
    }
}
