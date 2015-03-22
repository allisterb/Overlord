using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Security.ClaimTypes
{
    public class Resource
    {
        public const string Storage = "urn:Overlord/Identity/Claims/Storage";
    }
    public class Authentication
    {
        public const string Role = "urn:Overlord/Identity/Claims/Roles";
        public const string AdminUserId = "urn:Overlord/Identity/Claims/UserId";
        public const string AdminUserToken = "urn:Overlord/Identity/Claims/UserToken";
        public const string UserId = "urn:Overlord/Identity/Claims/UserId";
        public const string UserToken = "urn:Overlord/Identity/Claims/UserToken";
        public const string DeviceId = "urn:Overlord/Identity/Claims/DeviceId";
        public const string DeviceToken = "urn:Overlord/Identity/Claims/DeviceToken";
        
    }
}
