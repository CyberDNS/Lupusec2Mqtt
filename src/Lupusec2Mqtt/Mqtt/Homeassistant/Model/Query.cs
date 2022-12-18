using Lupusec2Mqtt.Lupusec;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt.Homeassistant.Model
{
    public class Query
    {
        public string Name { get; }

        public string ValueTopic { get; }

        public Func<ILogger, ILupusecService, Task<string>> GetValue { get; }

        public Query(string name, string valueTopic, Func<ILogger, ILupusecService, Task<string>> getValue)
        {
            Name = name;
            ValueTopic = EscapeTopic(valueTopic);
            GetValue = getValue;
        }

        private string EscapeTopic(string topic)
        {
            return topic.Replace(":", "_");
        }
    }
}
