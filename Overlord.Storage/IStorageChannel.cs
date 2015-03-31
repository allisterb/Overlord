using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    public class IStorageChannel
    {
        public Guid Id { get; set; }
        public string ETag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SensorType { get; set; }
        public List<IStorageAlert> Alerts { get; set; }
    }
}
