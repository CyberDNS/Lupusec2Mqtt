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

        public virtual string ConfigTopic => EscapeTopic($"homeassistant/{Component}/lupusec/{GetStaticValue<string>("unique_id")}/config");

        private Dictionary<string, object> _staticValues = new Dictionary<string, object>();
        public IEnumerable<StaticValue> StaticValues { get => _staticValues.Select(kvp => new StaticValue(kvp.Key, kvp.Value)); }

        private Dictionary<string, object> _deviceInfo = new Dictionary<string, object>();
        public IEnumerable<StaticValue> DeviceInfo { get => _deviceInfo.Select(kvp => new StaticValue(kvp.Key, kvp.Value)); }

        private List<Query> _queries = new List<Query>();
        public IEnumerable<Query> Queries { get => _queries.ToArray(); }


        private List<Command> _commands = new List<Command>();
        public IEnumerable<Command> Commands { get => _commands.ToArray(); }

        protected void DeclareStaticValue(string name, object value)
        {
            _staticValues.Add(name, value);
        }

        protected void DeclareDeviceInfo(string name, object value)
        {
            _deviceInfo.Add(name, value);
        }

        protected void DeclareQuery(string name, string valueTopic, Func<ILogger, ILupusecService, Task<string>> getValue)
        {
            _queries.Add(new Query(name, valueTopic, getValue));
        }

        protected void DeclareCommand(string name, string commandTopic, Func<ILogger, ILupusecService, string, Task> executeCommand)
        {
            _commands.Add(new Command(name, commandTopic, executeCommand));
        }

        public T GetStaticValue<T>(string key)
        {
            return (T)_staticValues[key];
        }

        protected string EscapeTopic(string topic)
        {
            return topic.Replace(":", "_");
        }

        protected void DeclareLupusXT1PlusDevice()
        {
            DeclareDeviceInfo("identifiers", new[] { "lupus_xt1_plus" });
            DeclareDeviceInfo("name", "Lupus XT1 Plus");
            DeclareDeviceInfo("model", "XT1 Plus");
            DeclareDeviceInfo("manufacturer", "Lupus Electronics");
        }

        public override string ToString()
        {
            return $"{GetStaticValue<string>("unique_id")} - {GetStaticValue<string>("name")}";
        }
    }
}
