using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    class IStorageChannelItem
    {
        public DateTime Time { get; set; }
        public IDictionary<string, object> SensorValues { get; set; }
    }
}
