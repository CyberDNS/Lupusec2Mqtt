using Lupusec2Mqtt.Lupusec;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Model
{
    public class Command
    {
        public string Name { get; }

        public string CommandTopic { get; }

        public Func<ILogger, ILupusecService, string, Task> ExecuteCommand { get; }

        public Command(string name, string commandTopic, Func<ILogger, ILupusecService, string, Task> executeCommand)
        {
            Name = name;
            CommandTopic = EscapeTopic(commandTopic);
            ExecuteCommand = executeCommand;
        }

        private string EscapeTopic(string topic)
        {
            return topic.Replace(":", "_");
        }
    }
}
