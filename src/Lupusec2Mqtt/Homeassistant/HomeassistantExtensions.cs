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
            Console.WriteLine($"Homeassistant config file path is {path}");

            if (File.Exists(path))
            {
                Console.WriteLine("Config is:");
                Console.WriteLine(File.ReadAllText(path));

                return builder.Add(new HomeassistantConfigurationSource(path));
            }

            if (path != null) { Console.WriteLine($"Homeassistant config file not found at {path}"); }

            return builder;
        }
    }
}
