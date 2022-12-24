using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt
{
    public interface IMqttService
    {
        Task StartAsync();
        void Register(string topic, Func<string, Task> callback);
        Task PublishAsync(string topic, string payload);
        Task StopAsync();
    }
}