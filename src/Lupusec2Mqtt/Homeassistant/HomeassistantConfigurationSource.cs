using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lupusec2Mqtt.Homeassistant
{
    public class HomeassistantConfigurationSource : IConfigurationSource
    {
        private readonly string _path;

        public HomeassistantConfigurationSource(string path)
        {
            _path = path;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {

            return new HomeassistantConfigurationProvider(_path);
        }
    }
}
