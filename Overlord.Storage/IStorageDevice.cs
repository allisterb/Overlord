using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    public class IStorageDevice
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Sensors { get; set; }
    }
}
