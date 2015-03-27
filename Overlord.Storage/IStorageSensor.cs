using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Overlord.Storage
{
    public class IStorageSensor
    {
        public Guid DeviceId { get; set; }
        public string Name { get; set; }        
        public string Unit { get; set; }
        public IList<Guid> Channels { get; set; }
        public IList<Guid> Alerts { get; set; }            
    }
    
}
