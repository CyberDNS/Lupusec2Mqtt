namespace Lupusec2Mqtt.Mqtt.Homeassistant.Model
{
    public class StaticValue
    {
        public string Name { get; }

        public object Value { get; }

        public StaticValue(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
