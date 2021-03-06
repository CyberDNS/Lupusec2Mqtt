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
            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            SensorList responseBody = await response.Content.ReadAsAsync<SensorList>();

            return responseBody;
        }

        public async Task<RecordList> GetRecordsAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/recordListGet");
            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            RecordList responseBody = await response.Content.ReadAsAsync<RecordList>();

            return responseBody;
        }

        public async Task<PowerSwitchList> GetPowerSwitches()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/deviceListPSSGet");
            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            PowerSwitchList responseBody = await response.Content.ReadAsAsync<PowerSwitchList>();

            return responseBody;
        }

        public async Task<PanelCondition> GetPanelConditionAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/panelCondGet");
            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            PanelCondition responseBody = await response.Content.ReadAsAsync<PanelCondition>();

            return responseBody;
        }

        public async Task<ActionResult> SetAlarmMode(int area, AlarmMode mode)
        {
            ActionResult responseBody = null;
            try
            {
                IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>> {
                    { new KeyValuePair<string, string>("area", $"{area}") },
                    { new KeyValuePair<string, string>("mode", $"{(byte)mode}") },
                };

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/panelCondPost");
                request.Content = new FormUrlEncodedContent(formData);

                HttpResponseMessage response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();

                responseBody = await response.Content.ReadAsAsync<ActionResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured in SetAlarmMode");
            }

            return responseBody;
        }

        public async Task<ActionResult> SetSwitch(string uniqueId, bool onOff)
        {
            ActionResult responseBody = null;
            try
            {
                IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>> {
                    { new KeyValuePair<string, string>("switch", $"{(onOff ? 1 : 0)}") },
                    { new KeyValuePair<string, string>("pd", string.Empty) },
                    { new KeyValuePair<string, string>("id", $"{uniqueId}") },
                };

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/deviceSwitchPSSPost");
                request.Content = new FormUrlEncodedContent(formData);

                HttpResponseMessage response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();

                responseBody = await response.Content.ReadAsAsync<ActionResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured in SetSwitch");
            }

            return responseBody;
        }
    }
}