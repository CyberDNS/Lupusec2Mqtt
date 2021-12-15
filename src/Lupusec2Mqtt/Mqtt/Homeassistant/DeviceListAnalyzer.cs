using System;
using System.Collections;
using System.Collections.Generic;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt.Homeassistant.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lupusec2Mqtt.Mqtt.Homeassistant
{
    class DeviceListAnalyzer{
        private readonly IConfiguration _configuration;

        private readonly ILogger _logger;

        public DeviceListAnalyzer(IConfiguration configuration,ILogger logger)
        {
            _logger=logger;
            this._configuration = configuration;
        }
        public IEnumerable<IStateProvider> Analyze(Sensor sensor)
        {
            List<IStateProvider> list = new List<IStateProvider>();

            switch (sensor.TypeId)
            {
                case 4: // Opener contact
                case 9: // Motion detector
                case 11: // Smoke detector
                case 5: // Water detector
                    list.Add(new BinarySensor(_configuration, sensor));
                    break;
                case 54: // Temperature/Humidity detector
                    list.Add(new TemperatureSensor(_configuration, sensor));
                    list.Add(new HumiditySensor(_configuration, sensor));
                    break;
                default:
                    _logger.LogWarning("Sensor of Type {typeId} is not supported. All Information about this unsupported Sensor:\n{sensor}",sensor.TypeId,sensor);
                    break;
            }

            return list;
        }
    }
}
