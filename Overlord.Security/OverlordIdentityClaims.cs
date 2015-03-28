using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlord.Security.Claims
{
    public class UserRole
    {

        public const string Administrator = "urn:Overlord/Identity/Roles/Administrator";

        public const string User = "urn:Overlord/Identity/Roles/User";

        public const string Device = "urn:Overlord/Identity/Roles/Device";

        public const string Anonymous = "urn:Overlord/Identity/Roles/Anonymous";

    }

    public class StorageAction
    {
        public const string AddUser = "urn:Overlord/Identity/Claims/User/Create";
        public const string UpdateUser = "urn:Overlord/Identity/Claims/User/Update";
        public const string FindUser = "urn:Overlord/Identity/User/Device/Retrieve";
        public const string DeleteUser = "urn:Overlord/Identity/User/Device/Delete"; 
     
        public const string AddDevice = "urn:Overlord/Identity/Claims/Device/Create";
        public const string UpdateDevice = "urn:Overlord/Identity/Claims/Device/Update";
        public const string FindDevice = "urn:Overlord/Identity/Claims/Device/Retrieve";
        public const string DeleteDevice = "urn:Overlord/Identity/Claims/Device/Delete";

        public const string AddSensor = "urn:Overlord/Identity/Claims/Sensor/Create";

        public const string AddDeviceReading = "urn:Overlord/Identity/Claims/DeviceReading/Create";
        public const string GetDeviceReading = "urn:Overlord/Identity/Claims/DeviceReading/Retrieve";

        public const string AddChannel = "urn:Overlord/Identity/Claims/Channel/Create";

        public const string AddMessage = "urn:Overlord/Identity/Claims/Channel/Create";

        public const string AddAlert = "urn:Overlord/Identity/Claims/Channel/Create";
    }
}
