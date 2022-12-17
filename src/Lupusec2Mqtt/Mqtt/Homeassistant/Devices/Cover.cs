using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Lupusec;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using Serilog;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class Cover : Device, IDevice, IStateProvider, IPositionProvider, ICommandable
    {
        protected readonly Sensor _shutter;

        [JsonProperty("state_topic")]
        public string StateTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/state");

        [JsonProperty("position_topic")]
        public string PositionTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/position");

        [JsonProperty("command_topic")]
        public string CommandTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/set");

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
            // TODO: Implement call to Lupusec
        }

    }
}
