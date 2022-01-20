using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lupusec2Mqtt.Lupusec
{
    public class LupusecService : ILupusecService
    {
        private readonly ILogger<LupusecService> _logger;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public LupusecService(ILogger<LupusecService> logger, HttpClient client, IConfiguration configuration)
        {
            _logger = logger;
            _client = client;
            _configuration = configuration;
        }

        public async Task<SensorList> GetSensorsAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/deviceListGet");
            SensorList responseBody = await SendRequest<SensorList>(request);
            return responseBody;
        }

        public async Task<RecordList> GetRecordsAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/recordListGet");
            RecordList responseBody = await SendRequest<RecordList>(request);
            return responseBody;
        }

        public async Task<PowerSwitchList> GetPowerSwitches()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/deviceListPSSGet");
            PowerSwitchList responseBody = await SendRequest<PowerSwitchList>(request);
            return responseBody;
        }

        public async Task<PanelCondition> GetPanelConditionAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/panelCondGet");
            PanelCondition responseBody = await SendRequest<PanelCondition>(request);
            return responseBody;
        }

        public async Task<ActionResult> SetAlarmMode(int area, AlarmMode mode)
        {
            IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("area", $"{area}") },
                { new KeyValuePair<string, string>("mode", $"{(byte)mode}") },
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/panelCondPost");
            request.Content = new FormUrlEncodedContent(formData);

            ActionResult responseBody = await SendRequest<ActionResult>(request);

            return responseBody;
        }

        public async Task<ActionResult> SetSwitch(string uniqueId, bool onOff)
        {
            IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("switch", $"{(onOff ? 1 : 0)}") },
                { new KeyValuePair<string, string>("pd", string.Empty) },
                { new KeyValuePair<string, string>("id", $"{uniqueId}") },
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/deviceSwitchPSSPost");
            request.Content = new FormUrlEncodedContent(formData);

            ActionResult responseBody = await SendRequest<ActionResult>(request);

            return responseBody;
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request)
        {
            try
            {
                HttpResponseMessage response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                T responseBody = await response.Content.ReadAsAsync<T>();
                _logger.LogInformation("This was the Answer for requesting {uri}:\n{body}", request.RequestUri, responseBody);
                return responseBody;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Call to Lupusec has an error! Request was:\n{request}", request);
            }
            return default(T);
        }
    }
}
