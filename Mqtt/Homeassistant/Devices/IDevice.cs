namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public interface IDevice
    {
        string Name { get; }
        string UniqueId { get; }

        string ConfigTopic { get; }


    }
}