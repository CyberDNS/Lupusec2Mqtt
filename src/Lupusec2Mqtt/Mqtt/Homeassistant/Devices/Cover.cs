using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Lupusec;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using Serilog;
using System.ComponentModel.Design;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class Cover : Device, IDevice, IStateProvider, IPositionProvider
    {
        protected readonly Sensor _shutter;

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonProperty("position_topic")]
        public string PositionTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/position");

        [JsonProperty("position_open")]
        public int PositionOpen => 100;

        [JsonProperty("position_closed")]
        public int PositionClosed => 0;

        [JsonProperty("set_position_topic")]
        public string PositionCommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set-pos");

        [JsonProperty("command_topic")]
        public string StateCommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set");

        protected override string _component => "cover";

        [JsonIgnore]
        public string State => GetState();

        [JsonIgnore]
        public int Position => GetPosition();

        public Cover(IConfiguration configuration, Sensor shutter)
            : base(configuration)
        {
            _shutter = shutter;

            UniqueId = shutter.SensorId;
            Name = GetValue(nameof(Name), shutter.Name);

            Commands = new Command[] 
            {
                new Command(StateCommandTopic, ExecuteCommand),
                new Command(PositionCommandTopic, ExecuteCommand),
            };

        }

        private string GetState()
        {
            if (_shutter.Level <= 0) { return "closed"; }
            else { return "open"; }
        }

        private int GetPosition()
        {
            return _shutter.Level;
        }

        public void ExecuteCommand(string command, ILupusecService lupusecService)
        {
            if (command.Equals("OPEN")) { command = "on"; }
            else if (command.Equals("CLOSE")) { command = "off"; }
            else if (command.Equals("STOP")) { command = "stop"; }

            lupusecService.SetCoverPosition(_shutter.Area, _shutter.Zone, command);
        }
    }
}
