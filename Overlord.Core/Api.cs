using System;
using System.Collections.Generic;
using System.Linq;

using System.Security;
using System.Security.Claims;
using System.Security.Permissions;

using System.Text;
using System.Threading.Tasks;

using Overlord.Security;
using Overlord.Security.ClaimTypes;
using Overlord.Security.Claims;

using Overlord.Storage;

using Overlord.Core.Models;

namespace Overlord.Core
{
    public static class Api
    {
        static Api()
        {
            OverlordIdentity.InitializeAnonymousIdentity();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.User)]
        public static void AddDevice(string user_token, DateTime time, string name, GeoIP geoip)
        {
            Guid uuid = new Guid();
            AzureStorage storage = new AzureStorage();
            //storage.AddDevice(time, uuid.ToString().ToString(), name, geoip);
        }
        
        public static void AddDeviceReading(DateTime reading_time, Dictionary<string, object> reading)
        {
            AzureStorage storage = new AzureStorage();
            //storage.A
        }

        public static void FindUser(string user_name, string user_token, GeoIP user_geo_ip)
        {
        }
    }
}
