using System.Collections;
using System.Collections.Generic;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public interface IDevice
    {
        string Name { get; }
        string UniqueId { get; }

        string ConfigTopic { get; }

        IEnumerable<Command> Commands { get; }

    }
}