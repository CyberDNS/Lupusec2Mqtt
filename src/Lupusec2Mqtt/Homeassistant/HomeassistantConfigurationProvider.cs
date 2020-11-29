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
                if (kvp.Value is IList)
                {
                    foreach (var listItem in ((IEnumerable)kvp.Value).Cast<ExpandoObject>())
                    {
                        string key = kvp.Key;
                        string uniqueId = listItem.Where(e => e.Key.Equals("UniqueId", StringComparison.OrdinalIgnoreCase)).Select(e => e.Value.ToString()).SingleOrDefault();
                        
                        if (uniqueId != null) { key += $":{uniqueId}"; }

                        foreach (var listValue in listItem.Where(e => !e.Key.Equals("UniqueId", StringComparison.OrdinalIgnoreCase)))
                        {
                            Data.Add($"{key}:{listValue.Key}", listValue.Value.ToString());
                        }
                    }
                }
                else
                {
                    Data.Add(kvp.Key, kvp.Value.ToString());
                }
            }
        }

    }
}
