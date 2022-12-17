using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public abstract class Device : IDevice
    {

        protected readonly IConfiguration _configuration;

        [JsonProperty("name")]
        public string Name { get; protected set; }

        [JsonProperty("unique_id")]
        public string UniqueId { get; protected set; }

        protected abstract string _component { get; }

        [JsonIgnore]
        public virtual string ConfigTopic => EscapeTopic($"homeassistant/{_component}/lupusec/{UniqueId}/config");

        [JsonIgnore]
        public IEnumerable<Command> Commands { get; protected set; } = Array.Empty<Command>();

        public Device(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected string GetValue(string property, string defaultValue)
        {
            return _configuration[$"Mappings:{UniqueId}:{property}"] ?? defaultValue;
        }

        protected string EscapeTopic(string topic)
        {
            return topic.Replace(":", "_");
        }
    }
}