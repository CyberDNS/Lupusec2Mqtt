namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public interface IDevice
    {
        string Name { get; set; }
        string UniqueId { get; set; }

        string ConfigTopic { get; }


    }
}