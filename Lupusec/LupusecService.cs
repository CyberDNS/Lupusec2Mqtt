using System.Net.Http;
using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec.Dtos;
using Newtonsoft.Json;

namespace Lupusec2Mqtt.Lupusec
{
    public class LupusecService : ILupusecService
    {
        private readonly HttpClient _client;

        public LupusecService(HttpClient client)
        {
            _client = client;
        }


        public async Task<SensorList> GetSensorsAsync()
        {
            HttpResponseMessage response = await _client.PostAsync("/action/deviceListGet", null);
            response.EnsureSuccessStatusCode();
            SensorList responseBody = await response.Content.ReadAsAsync<SensorList>();

            return responseBody;
        }

        public async Task<PanelCondition> GetPanelConditionAsync()
        {
            HttpResponseMessage response = await _client.PostAsync("/action/panelCondGet", null);
            response.EnsureSuccessStatusCode();
            PanelCondition responseBody = await response.Content.ReadAsAsync<PanelCondition>();

            return responseBody;
        }
    }
}