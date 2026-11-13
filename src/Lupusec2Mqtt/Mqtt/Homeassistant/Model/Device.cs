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

        public const string LupusecDeviceIdentifier = "lupusec";

        /// <summary>
        /// Attaches the entity to the Lupusec hub device (used for panel/area level entities).
        /// </summary>
        protected void DeclareLupusecDevice()
        {
            DeclareDeviceInfo("identifiers", new[] { LupusecDeviceIdentifier });
            DeclareDeviceInfo("name", "Lupusec");
            DeclareDeviceInfo("manufacturer", "Lupus Electronics");
            // Supported alarm centrals (see README). Kept out of the device name so entity names stay short.
            DeclareDeviceInfo("model", "XT1 Plus / XT2 / XT2 Plus");
        }

        /// <summary>
        /// Attaches the entity to its own Home Assistant device representing a physical Lupusec
        /// sensor or switch, linked to the Lupusec hub device via "via_device".
        /// Entities created for the same physical device (e.g. switch + power + energy) share the same sub-device.
        /// </summary>
        protected void DeclareLupusecSubDevice(string id, string name, int typeId)
        {
            DeclareDeviceInfo("identifiers", new[] { EscapeTopic($"{LupusecDeviceIdentifier}_{id}") });
            DeclareDeviceInfo("name", name);
            DeclareDeviceInfo("manufacturer", "Lupus Electronics");
            DeclareDeviceInfo("model", _modelNames.TryGetValue(typeId, out var model) ? model : $"Type {typeId}");
            DeclareDeviceInfo("via_device", LupusecDeviceIdentifier);
        }

        // The panel only returns localization placeholders (e.g. "{D_TYPE_48}") as type names,
        // so map the type ids handled by the device factories to readable model names.
        private static readonly Dictionary<int, string> _modelNames = new()
        {
            { 4, "Door/Window Contact" },
            { 5, "Water Detector" },
            { 9, "Motion Detector" },
            { 11, "Smoke Detector" },
            { 24, "Power Switch" },
            { 33, "Door/Window Contact" },
            { 48, "Power Switch Meter" },
            { 54, "Temperature/Humidity Sensor" },
            { 57, "Door Lock" },
            { 74, "Dimmer" },
            { 76, "Shutter Relay" },
            { 79, "Radiator Thermostat" },
            { 93, "Vibration Detector" },
        };

        public override string ToString()
        {
            var name = GetStaticValue<string>("name");
            if (name == null && _deviceInfo.TryGetValue("name", out var deviceName)) { name = deviceName as string; }
            return $"{GetStaticValue<string>("unique_id")} - {name}";
        }
    }
}
