using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Lupusec2Mqtt.Lupusec.Dtos;

namespace Lupusec2Mqtt.Lupusec
{
    public class LupusecTokenHandler : DelegatingHandler
    {
        static private string _token; // Same token for all
        private readonly HttpClient _client;
        public LupusecTokenHandler(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _token = _token ?? await GetToken();

            request.Headers.Add("X-Token", _token);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetToken()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/action/tokenGet");
            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            ActionResult responseBody = await response.Content.ReadAsAsync<ActionResult>();

            return responseBody.Message;
        }
    }
}