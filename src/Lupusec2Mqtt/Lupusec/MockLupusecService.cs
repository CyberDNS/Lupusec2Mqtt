using Lupusec2Mqtt.Lupusec.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Reflection.Metadata.Ecma335;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Dynamic;
using Microsoft.JSInterop.Infrastructure;
using System.Text.Json;

namespace Lupusec2Mqtt.Lupusec
{
    public class MockLupusecService : ILupusecService
    {
        private readonly ILogger<LupusecService> _logger;
        private readonly IConfiguration _configuration;
        private readonly LupusecCache _cache;
        private readonly HttpClient _client;

        public SensorList SensorList => _cache.SensorList;
        public SensorList SensorList2 => _cache.SensorList2;

        public RecordList RecordList => _cache.RecordList;

        public PowerSwitchList PowerSwitchList => _cache.PowerSwitchList;

        public PanelCondition PanelCondition => _cache.PanelCondition;

        public MockLupusecService(ILogger<LupusecService> logger, HttpClient client, IConfiguration configuration, LupusecCache cache)
        {
            _logger = logger;
            _configuration = configuration;
            _cache = cache;
            _client = client;
        }

        public Task<SensorList> GetSensorsAsync()
        {
            var content = GetMockFileContent("sensors");
            return Task.FromResult(content is not null ? JsonConvert.DeserializeObject<SensorList>(content) : new SensorList());
        }

        public Task<SensorList> GetSensors2Async()
        {
            var content = GetMockFileContent("sensors2");
            return Task.FromResult(content is not null ? JsonConvert.DeserializeObject<SensorList>(content) : new SensorList());
        }

        public Task<RecordList> GetRecordsAsync()
        {
            var content = GetMockFileContent("records");
            return Task.FromResult(content is not null ? JsonConvert.DeserializeObject<RecordList>(content) : new RecordList());
        }

        public Task<PowerSwitchList> GetPowerSwitches()
        {
            var content = GetMockFileContent("power-switch");
            return Task.FromResult(content is not null ? JsonConvert.DeserializeObject<PowerSwitchList>(content) : new PowerSwitchList());
        }

        public Task<PanelCondition> GetPanelConditionAsync()
        {
            var content = GetMockFileContent("panel-condition");
            return Task.FromResult(content is not null ? JsonConvert.DeserializeObject<PanelCondition>(content) : new PanelCondition());
        }

        public async Task PollAllAsync()
        {
            _cache.UpdateSensorList(await GetSensorsAsync());
            _cache.UpdateSensorList2(await GetSensors2Async());
            _cache.UpdateRecordList(await GetRecordsAsync());
            _cache.UpdatePowerSwitchList(await GetPowerSwitches());
            _cache.UpdatePanelCondition(await GetPanelConditionAsync());
        }

        //public async Task<LupusecResponseBody> SetAlarmMode(int area, AlarmMode mode)
        //{
        //    return new LupusecResponseBody();
        //}

        //public async Task<LupusecResponseBody> SetSwitch(string uniqueId, bool onOff)
        //{
        //    return new LupusecResponseBody();
        //}

        //public async Task<LupusecResponseBody> SetCoverPosition(byte area, byte zone, string command)
        //{
        //    return new LupusecResponseBody();
        //}

        private string GetMockFileContent(string type)
        {
            string path = Path.Combine(_configuration.GetValue<string>("Lupusec:MockFilesPath"), $"{type}.json");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return null;
        }





        public async Task<LupusecResponseBody> SetAlarmMode(int area, AlarmMode mode)
        {
            IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("area", $"{area}") },
                { new KeyValuePair<string, string>("mode", $"{(byte)mode}") },
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/panelCondPost");
            request.Content = new FormUrlEncodedContent(formData);

            LupusecResponseBody responseBody = await SendRequest<LupusecResponseBody>(request, LogLevel.Debug);


            return responseBody;
        }

        public async Task<LupusecResponseBody> SetSwitch(string uniqueId, bool onOff)
        {
            IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("switch", $"{(onOff ? 1 : 0)}") },
                { new KeyValuePair<string, string>("pd", string.Empty) },
                { new KeyValuePair<string, string>("id", $"{uniqueId}") },
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/deviceSwitchPSSPost");
            request.Content = new FormUrlEncodedContent(formData);

            LupusecResponseBody responseBody = await SendRequest<LupusecResponseBody>(request, LogLevel.Debug);

            return responseBody;
        }

        public async Task<LupusecResponseBody> SetCoverPosition(byte area, byte zone, string command)
        {
            IList<KeyValuePair<string, string>> formData = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("a", area.ToString()) },
                { new KeyValuePair<string, string>("z", zone.ToString()) },
                { new KeyValuePair<string, string>("shutter", command) },
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/action/haExecutePost");

            var content = new FormUrlEncodedContent(formData);
            var formUrlEncodedString = await content.ReadAsStringAsync();

            request.Content = new FormUrlEncodedContent(new[] { KeyValuePair.Create( "exec", formUrlEncodedString) });

            LupusecResponseBody responseBody = await SendRequest<LupusecResponseBody>(request, LogLevel.Debug);

            return responseBody;
        }

        private async Task<T> SendRequest<T>(HttpRequestMessage request, LogLevel logLevel = LogLevel.Trace)
        {
            try
            {
                string requestBody = null;
                if (request.Content is not null)
                {
                    requestBody = await request.Content.ReadAsStringAsync();
                }

                _logger.Log(logLevel, "Request for {Method} {Uri}:\nRequest:\n{Request}\nRequest body:\n{Body}", request.Method, request.RequestUri, request, requestBody);

                HttpResponseMessage response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                T responseBody = await response.Content.ReadAsAsync<T>();

                _logger.Log(logLevel, "Response for {Method} {Uri}:\nResponse:\n{Response}\nResponse body:\n{Body}", request.Method, request.RequestUri, response, responseBody);
                return responseBody;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling {Method} {Uri}:\nRequest:\n{Request}", request.Method, request.RequestUri, request);
            }
            return default(T);
        }

        public Task<LupusecResponseBody> SetThermostatMode(string uniqueId, ThermostateMode mode)
        {
            throw new NotImplementedException();
        }
        
        public Task<LupusecResponseBody> SetThermostatTemperature(string uniqueId, int destinationtTemperature)
        {
            throw new NotImplementedException();
        }
    }
}
