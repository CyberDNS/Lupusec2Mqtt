using System.Text;
using Microsoft.Extensions.Configuration;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Lupusec2Mqtt.Mqtt
{
    public class MqttService
    {
        private readonly MqttClient _client;
        public MqttService(IConfiguration configuration)
        {
            _client = new MqttClient(configuration["Mqtt:Server"]);
            _client.Connect("Lupusec2Mqtt", configuration["Mqtt:Login"], configuration["Mqtt:Password"]);
        }

        public void Publish(string topic, string payload)
        {
            _client.Publish(topic, Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
        }
    }
}