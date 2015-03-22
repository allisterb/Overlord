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
        public IList<IStorageChannel> Channels { get; set; }
        public IList<IStorageAlert> Alerts { get; set; }
    }

    public class IStorageSensorEq : IEqualityComparer<IStorageSensor>
    {
        public bool Equals(IStorageSensor s1, IStorageSensor s2)
        {
            if ((s1.Id == s2.Id) && (s1.Name == s2.Name) && (s1.Version == s2.Version))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(IStorageSensor s)
        {
            return (s.Id + s.Version).GetHashCode();
        }
    }
}
