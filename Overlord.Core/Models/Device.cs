using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Core.Models
{
    class Device
    {
        public Guid Uuid { get; set; }
        public string AuthenticationToken { get; set; }
        public GeoIP GeoIP { get; set; }

        public string Name { get; set; }
    }
}
