namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public interface IPositionProvider : IDevice
    {
        int Position { get; }

        string PositionTopic { get; }
    }
}
