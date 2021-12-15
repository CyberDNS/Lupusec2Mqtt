﻿using Lupusec2Mqtt.Lupusec;
using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class Light : Device, IStateProvider, ISettable
    {
        protected readonly PowerSwitch _powerSwitch;

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonProperty("command_topic")]
        public string CommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set");

        protected override string _component => "light";

        [JsonIgnore]
        public string State => GetState();

        private string GetState()
        {
            return _powerSwitch.Status.Contains("{WEB_MSG_DIMMER_ON}") ? "ON" : "OFF";
        }

        public void SetState(string state, ILupusecService lupusecService)
        {
            lupusecService.SetSwitch(UniqueId, state.Equals("on", StringComparison.OrdinalIgnoreCase));
        }


        public Light(IConfiguration configuration, PowerSwitch powerSwitch)
        : base(configuration)
        {
            _powerSwitch = powerSwitch;

            UniqueId = _powerSwitch.Id;
            Name = GetMappingValue(nameof(Name), _powerSwitch.Name);
        }
    }
}
