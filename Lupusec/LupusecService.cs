using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec.Dtos;


namespace Lupusec2Mqtt.Lupusec
{
    public class LupusecService : ILupusecService
    {


        private readonly HttpClient _client;

        private string _token;
        private DateTime _tokenLastRefresh;
        private readonly TimeSpan _tokenRefreshRate = TimeSpan.FromMinutes(5);

        public LupusecService(HttpClient client)
        {
            _client = client;
        }

        private async Task UpdateToken()
        {
            if ((_tokenLastRefresh == default(DateTime)) || (DateTime.UtcNow - _tokenLastRefresh) > _tokenRefreshRate)
            {
                HttpResponseMessage response = await _client.GetAsync("/action/tokenGet");
                response.EnsureSuccessStatusCode();

                ActionResult responseBody = await response.Content.ReadAsAsync<ActionResult>();

                _token = responseBody.Message;
                _tokenLastRefresh = DateTime.UtcNow;

                _client.DefaultRequestHeaders.Add("X-Token", _token);
            }
        }

        public async Task<SensorList> GetSensorsAsync()
        {
            await UpdateToken();

            HttpResponseMessage response = await _client.PostAsync("/action/deviceListGet", null);
            response.EnsureSuccessStatusCode();
            SensorList responseBody = await response.Content.ReadAsAsync<SensorList>();

            return responseBody;
        }

        public async Task<PanelCondition> GetPanelConditionAsync()
        {
            await UpdateToken();

            HttpResponseMessage response = await _client.PostAsync("/action/panelCondGet", null);
            response.EnsureSuccessStatusCode();
            PanelCondition responseBody = await response.Content.ReadAsAsync<PanelCondition>();

            return responseBody;
        }

        public async Task<ActionResult> SetAlarmMode(int area, AlarmMode mode)
        {
            await UpdateToken();

            IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("area", $"{area}") },
                { new KeyValuePair<string, string>("mode", $"{(byte)mode}") },
            };

            HttpResponseMessage response = await _client.PostAsync("/action/panelCondPost", new FormUrlEncodedContent(formData));
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();

            ActionResult responseBody = await response.Content.ReadAsAsync<ActionResult>();

            return responseBody;
        }
    }
}