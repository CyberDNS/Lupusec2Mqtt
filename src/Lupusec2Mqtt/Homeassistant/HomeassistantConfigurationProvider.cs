using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Homeassistant
{
    public class HomeassistantConfigurationProvider : ConfigurationProvider
    {
        private readonly string _path;

        public HomeassistantConfigurationProvider(string path)
        {
            _path = path;
            var expando = JsonConvert.DeserializeObject<ExpandoObject>(File.ReadAllText(_path));

            foreach(var kvp in expando)
            {
                Data.Add(kvp.Key, kvp.Value.ToString());
            }
        }

    }
}
