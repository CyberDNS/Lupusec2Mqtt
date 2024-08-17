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
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Token might be expired, reset and get a new one
                ResetToken();
                _token = await GetToken();

                // Retry the request with the new token
                request.Headers.Remove("X-Token");
                request.Headers.Add("X-Token", _token);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }

        private async Task<string> GetToken()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/action/tokenGet");
            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            LupusecResponseBody responseBody = await response.Content.ReadAsAsync<LupusecResponseBody>();

            return responseBody.Message;
        }

        static public void ResetToken()
        {
            _token = null;
        }
    }
}