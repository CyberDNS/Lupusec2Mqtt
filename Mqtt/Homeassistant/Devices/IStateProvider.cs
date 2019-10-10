namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public interface IStateProvider : IDevice
    {
        string State { get; }

        string StateTopic { get; }
    }
}