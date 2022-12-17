using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class Lock : Device, IDevice, IStateProvider
    {
        protected readonly PowerSwitch _powerSwitch;

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonProperty("command_topic")]
        public string CommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set");

        protected override string _component => "lock";

        [JsonIgnore]
        public string State => GetState();

        public Lock(IConfiguration configuration, PowerSwitch powerSwitch)
            : base(configuration)
        {
            _powerSwitch = powerSwitch;

            UniqueId = _powerSwitch.Id;
            Name = GetValue(nameof(Name), _powerSwitch.Name);

            Commands = new Command[]
            {
                new Command(CommandTopic, ExecuteCommand)
            };
        }

        private string GetState()
        {
            if (_powerSwitch.Status.Contains("{WEB_MSG_DL_LOCKED}"))
            {
                return "LOCKED";
            }
            else
            {
                return "UNLOCKED";
            }
        }

        public void ExecuteCommand(string state, ILupusecService lupusecService)
        {
            lupusecService.SetSwitch(UniqueId, state.Equals("LOCK", StringComparison.OrdinalIgnoreCase));
        }
    }
}
