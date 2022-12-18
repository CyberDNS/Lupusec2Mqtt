namespace Lupusec2Mqtt.Mqtt.Homeassistant.Model
{
    public class StaticValue
    {
        public string Name { get; }

        public string Value { get; }

        public StaticValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
