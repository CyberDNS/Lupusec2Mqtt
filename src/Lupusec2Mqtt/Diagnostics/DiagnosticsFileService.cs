using Lupusec2Mqtt.Lupusec;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Diagnostics
{
    public class DiagnosticsFileService
    {
        private readonly ILupusecService _lupusecService;

        public DiagnosticsFileService(ILupusecService lupusecService)
        {
            _lupusecService = lupusecService;
        }

        // Generate a zip file with all diagnostics data and provide it as a stream
        public async Task<Stream> GenerateDiagnosticsFileAsync()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var stream = new MemoryStream();
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                var diagnosticsData = new List<DiagnosticsData>
                {
                    new DiagnosticsData
                    {
                        Name = "SensorList",
                        Data = JsonConvert.SerializeObject(await _lupusecService.GetSensorsAsync(), settings)
                    },
                    new DiagnosticsData
                    {
                        Name = "SensorList2",
                        Data = JsonConvert.SerializeObject(await _lupusecService.GetSensors2Async(), settings)
                    },
                    new DiagnosticsData
                    {
                        Name = "RecordList",
                        Data = JsonConvert.SerializeObject(await _lupusecService.GetRecordsAsync(), settings)
                    },
                    new DiagnosticsData
                    {
                        Name = "PowerSwitchList",
                        Data = JsonConvert.SerializeObject(await _lupusecService.GetPowerSwitches(), settings)
                    },
                    new DiagnosticsData
                    {
                        Name = "PanelCondition",
                        Data = JsonConvert.SerializeObject(await _lupusecService.GetPanelConditionAsync(), settings)
                    }
                };
                foreach (var diagnostics in diagnosticsData)
                {
                    var entry = archive.CreateEntry($"{diagnostics.Name}.json");
                    using (var entryStream = entry.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        await streamWriter.WriteAsync(diagnostics.Data);
                    }
                }
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private class DiagnosticsData
        {
            public string Name { get; set; }
            public string Data { get; set; }
        }
    }
}
