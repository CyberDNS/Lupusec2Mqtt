using Lupusec2Mqtt.Lupusec;
using System;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Devices
{
    public class Command
    {
        public string CommandTopic { get; }

        public Action<string, ILupusecService> CommandAction { get; }

        public Command(string commandTopic, Action<string, ILupusecService> commandAction)
        {
            CommandTopic = commandTopic;
            CommandAction = commandAction;
        }
    }
}
