using Microsoft.Extensions.Configuration;
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

            return builder;
        }
    }
}
