using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Overlord.Core
{
    [DataContract]
    public class DeviceReadingModel
    {
        [DataMember]
        public DateTime Time { get; set; }
        
        public IDictionary<string, object> Values { get; set; }
    }
}
