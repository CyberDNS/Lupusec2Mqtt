using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Homeassistant
{
    public static class HomeassistantExtensions
    {
        public static IConfigurationBuilder AddHomeassistantConfig(this IConfigurationBuilder builder, string path)
        {
            if (File.Exists(path))
            {
                return builder.Add(new HomeassistantConfigurationSource(path));
            }

            if (path != null) { Log.Logger.Warning("Homeassistant config file not found at {homeassistantConfigFilePath}", path); }

            return builder;
        }
    }
}
