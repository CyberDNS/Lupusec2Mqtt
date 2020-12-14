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
        public static IConfigurationBuilder AddHomeassistantConfig(this IConfigurationBuilder builder, bool logging = false)
        {
            string path = @"/data/options.json";
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_HOMEASSISTANT__CONFIG"))) { path = Environment.GetEnvironmentVariable("ASPNETCORE_HOMEASSISTANT__CONFIG"); }

            if (logging) { Console.WriteLine($"Homeassistant config file path is {path}"); }

            if (File.Exists(path))
            {
                return builder.Add(new HomeassistantConfigurationSource(path));
            }

            if (path != null)
            {
                if (logging) { Console.WriteLine($"Homeassistant config file not found at {path}"); }
            }

            return builder;
        }
    }
}
