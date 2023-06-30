using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Mqtt
{
    public class MqttService : IMqttService
    {
        private IManagedMqttClient _managedMqttClient;
        private readonly IConfiguration _configuration;

        private Dictionary<string, Func<string, Task>> _registrations = new();

        public MqttService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task StartAsync()
        {
            var mqttFactory = new MqttFactory();

            _managedMqttClient = mqttFactory.CreateManagedMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_configuration["Mqtt:Server"], _configuration.GetValue("Mqtt:Port", 1883))
                .WithClientId("Lupusec2Mqtt")
                .WithCredentials(_configuration["Mqtt:Login"], _configuration["Mqtt:Password"])
                .Build();

            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .Build();

            _managedMqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedAsync;

            await _managedMqttClient.StartAsync(managedMqttClientOptions);
        }

        private async Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            if (_registrations.ContainsKey(arg.ApplicationMessage.Topic))
            {
                await _registrations[arg.ApplicationMessage.Topic].Invoke(arg.ApplicationMessage.ConvertPayloadToString());
            }
        }

        public void Register(string topic, Func<string, Task> callback)
        {
            if (!_registrations.ContainsKey(topic)) { _registrations.Add(topic, null); }
            _registrations[topic] = callback;

            _managedMqttClient.SubscribeAsync(topic);
        }

        public async Task PublishAsync(string topic, string payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(true)
                .Build();

            await _managedMqttClient.EnqueueAsync(message);
        }

        public async Task StopAsync()
        {
            await _managedMqttClient.StopAsync();
        }
    }
}
