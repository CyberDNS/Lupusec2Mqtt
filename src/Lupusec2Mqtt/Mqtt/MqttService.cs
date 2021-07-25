using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Lupusec2Mqtt.Mqtt
{
    public class MqttService
    {
        private readonly MqttClient _client;

        private IDictionary<string, Action<string>> _registrations = new Dictionary<string, Action<string>>();
        public MqttService(IConfiguration configuration)
        {

            _client = new MqttClient(configuration["Mqtt:Server"], configuration.GetValue("Mqtt:Port", 1883), false, null, null, MqttSslProtocols.None);

            _client.MqttMsgPublishReceived += MqttMsgPublishReceived;
            _client.Connect("Lupusec2Mqtt", configuration["Mqtt:Login"], configuration["Mqtt:Password"]);

            _client.Subscribe(new string[] { "/home/temperature" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        public void Publish(string topic, string payload)
        {
            _client.Publish(topic, Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
        }

        public void Register(string topic, Action<string> callback)
        {
            if (!_registrations.ContainsKey(topic)) { _registrations.Add(topic, null); }
            _registrations[topic] = callback;

            _client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        private void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (_registrations.ContainsKey(e.Topic))
            {
                _registrations[e.Topic].Invoke(Encoding.UTF8.GetString(e.Message));
            }
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }
    }
}