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
        private readonly LupusecCache _cache;

        public SensorList SensorList => _cache.SensorList;

        public RecordList RecordList => _cache.RecordList;

        public PowerSwitchList PowerSwitchList => _cache.PowerSwitchList;

        public PanelCondition PanelCondition => _cache.PanelCondition;

        public LupusecService(ILogger<LupusecService> logger, HttpClient client, IConfiguration configuration, LupusecCache cache)
        {
            _logger = logger;
            _client = client;
            _configuration = configuration;
            _cache=cache;
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

        public async Task PollAllAsync()
        {
            _cache.UpdateSensorList(await GetSensorsAsync());
            _cache.UpdateRecordList(await GetRecordsAsync());
            _cache.UpdatePowerSwitchList(await GetPowerSwitches());
            _cache.UpdatePanelCondition(await GetPanelConditionAsync());
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

        public async Task<ActionResult> SetCoverPosition(byte area, byte zone, string command)
        {
            IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("a", area.ToString()) },
                { new KeyValuePair<string, string>("z", zone.ToString()) },
                { new KeyValuePair<string, string>("shutter", command) },
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/haExecutePost");
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
                _logger.LogDebug("This was the Answer for requesting {uri}:\n{body}", request.RequestUri, responseBody);
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
