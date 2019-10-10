using System;
using Lupusec2Mqtt.Lupusec.Dtos;
using Lupusec2Mqtt.Mqtt.Homeassistant.Devices;
using Microsoft.Extensions.Configuration;

namespace Lupusec2Mqtt.Mqtt.Homeassistant
{
    public class ConversionService
    {
        private readonly IConfiguration _configuration;

        public ConversionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDevice GetDevice(Sensor sensor)
        {
            switch (sensor.TypeId)
            {
                case 4: // Opener contact
                case 9: // Motion detector
                case 11: // Smoke detector
                case 5: // Water detector
                    return new BinarySensor(_configuration, sensor);
                case 48: // Power meter switch
                    return null;
                default:
                    return null;
            }
        }

         public IStateProvider GetStateProvider(Sensor sensor)
        {
            switch (sensor.TypeId)
            {
                case 4: // Opener contact
                case 9: // Motion detector
                case 11: // Smoke detector
                case 5: // Water detector
                    return new BinarySensor(_configuration, sensor);
                case 48: // Power meter switch
                    return null;
                default:
                    return null;
            }
        }
    }
}