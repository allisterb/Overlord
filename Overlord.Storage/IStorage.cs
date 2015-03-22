using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Overlord.Storage.Common;

namespace Overlord.Storage
{
    public interface IStorage
    {
        IStorageUser AddUser(string user_name, string token, GeoIp geo_ip);
        IStorageUser FindUser(Guid id, string token);
        IStorageDevice AddDevice(IStorageUser user, string name, string token, GeoIp geoip);
    }
}
