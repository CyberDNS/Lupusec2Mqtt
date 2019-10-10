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
            // string responseBody = await _client.GetStringAsync("/action/deviceListGet");

            HttpResponseMessage response = await _client.PostAsync("/action/deviceListGet", null);
            response.EnsureSuccessStatusCode();
            SensorList responseBody = await response.Content.ReadAsAsync<SensorList>();

            return responseBody;
        }
    }
}