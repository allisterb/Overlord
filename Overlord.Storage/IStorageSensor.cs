using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    public class IStorageSensor
    {
        public Guid DeviceId { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public IEnumerable<IStorageChannel> Channels { get; set; }
        public IEnumerable<IStorageAlert> Alerts { get; set; }
    }
}
