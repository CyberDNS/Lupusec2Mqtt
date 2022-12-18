using System.Collections.Generic;
using System;
using Lupusec2Mqtt.Lupusec;
using System.Text.Json.Serialization;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Xml;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Model
{
    public abstract class Device
    {
        public abstract string Component { get; }

        public virtual string ConfigTopic => EscapeTopic($"homeassistant/{Component}/lupusec/{GetStaticValue("unique_id")}/config");

        private Dictionary<string, string> _staticValues = new Dictionary<string, string>();
        public IEnumerable<StaticValue> StaticValues { get => _staticValues.Select(kvp => new StaticValue(kvp.Key, kvp.Value)); }


        private List<Query> _queries = new List<Query>();
        public IEnumerable<Query> Queries { get => _queries.ToArray(); }


        private List<Command> _commands = new List<Command>();
        public IEnumerable<Command> Commands { get => _commands.ToArray(); }

        protected void DeclareStaticValue(string name, string value)
        {
            _staticValues.Add(name, value);
        }

        protected void DeclareQuery(string name, string valueTopic, Func<ILogger, ILupusecService, Task<string>> getValue)
        {
            _queries.Add(new Query(name, valueTopic, getValue));
        }

        protected void DeclareCommand(string name, string commandTopic, Func<ILogger, ILupusecService, string, Task> executeCommand)
        {
            _commands.Add(new Command(name, commandTopic, executeCommand));
        }

        public string GetStaticValue(string key)
        {
            return _staticValues[key];
        }

        protected string EscapeTopic(string topic)
        {
            return topic.Replace(":", "_");
        }

        public override string ToString()
        {
            return $"{GetStaticValue("unique_id")} - {GetStaticValue("name")}";
        }
    }
}
