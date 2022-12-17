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

namespace Lupusec2Mqtt.Lupusec
{
    public class MockLupusecService : ILupusecService
    {
        private readonly ILogger<LupusecService> _logger;
        private readonly IConfiguration _configuration;

        public MockLupusecService(ILogger<LupusecService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task<SensorList> GetSensorsAsync()
        {
            var content = GetMockFileContent("sensors");
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

        public async Task<ActionResult> SetAlarmMode(int area, AlarmMode mode)
        {
            return new ActionResult();
        }

        public async Task<ActionResult> SetSwitch(string uniqueId, bool onOff)
        {
            return new ActionResult();
        }

        public async Task<ActionResult> SetCoverPosition(byte area, byte zone, string command)
        {
            return new ActionResult();
        }

        private string GetMockFileContent(string type)
        {
            string path = Path.Combine(_configuration.GetValue<string>("Lupusec:MockFilesPath"), $"{type}.json");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return null;
        }


    }
}
