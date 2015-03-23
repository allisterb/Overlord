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
        IStorageUser UpdateUser(IStorageUser user);
        bool DeleteUser(IStorageUser user);

              
        IStorageDevice AddDevice(IStorageUser user, string name, string token, GeoIp location);
        IStorageDevice FindDevice(Guid id, string token);
        IStorageDevice FindDevice(); //Find the device associated with the current device indentity.
        IStorageDevice UpdateDevice(IStorageDevice device);

        IStorageSensor AddSensor(string sensor_name, string sensor_units,
            IList<Guid> sensor_channels, IList<Guid> sensor_alerts);

        IStorageChannel AddChannel(string channel_name, string channel_description,
            string channel_units);
    }
}
