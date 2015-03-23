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
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string ETag { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }
        public Common.GeoIp Location { get; set; }
        public IDictionary<string, IStorageSensor> Sensors { get; set; }
    }

    public class IStorageDeviceEq : IEqualityComparer<IStorageDevice>
    {
        public bool Equals(IStorageDevice d1, IStorageDevice d2)
        {
            if ((d1.Id == d2.Id) && (d1.Token == d2.Token) && (d1.ETag == d2.ETag))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(IStorageDevice d)
        {            
            return (d.Id + d.Token + d.ETag).GetHashCode();
        }
    }
}
