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
        public const string AddUser = "urn:Overlord/Identity/Claims/Storage/User/Create";
        public const string UpdateUser = "urn:Overlord/Identity/Claims/Storage/User/Update";
        public const string FindUser = "urn:Overlord/Identity/Claims/Storage/User/Retrieve";
        public const string DeleteUser = "urn:Overlord/Identity/Claims/User/Delete";

        public const string AddDevice = "urn:Overlord/Identity/Claims/Storage/Device/Create";
        public const string UpdateDevice = "urn:Overlord/Identity/Claims/Storage/Device/Update";
        public const string FindDevice = "urn:Overlord/Identity/Claims/Storage/Device/Retrieve";
        public const string DeleteDevice = "urn:Overlord/Identity/Claims/Storage/Device/Delete";

        public const string AddSensor = "urn:Overlord/Identity/Claims/Storage/Sensor/Create";

        public const string AddDeviceReading = "urn:Overlord/Identity/Claims/Storage/DeviceReading/Create";
        public const string GetDeviceReading = "urn:Overlord/Identity/Claims/Storage/DeviceReading/Retrieve";

        public const string AddChannel = "urn:Overlord/Identity/Claims/Storage/Channel/Create";

        public const string AddMessage = "urn:Overlord/Identity/Claims/Storage/Channel/Create";

        public const string AddAlert = "urn:Overlord/Identity/Claims/Storage/Channel/Create";
    }

    public class ApiAction
    {
        public const string AuthenticateUser = "urn:Overlord/Identity/Claims/Api/User/Authenticate";
        public const string AuthenticateDevice = "urn:Overlord/Identity/Claims/Api/Device/Authenticate";

        public const string AddUser = "urn:Overlord/Identity/Claims/Api/User/Create";
        public const string UpdateUser = "urn:Overlord/Identity/Claims/Api/User/Update";
        public const string FindUser = "urn:Overlord/Identity/Claims/Api/User/Retrieve";
        public const string DeleteUser = "urn:Overlord/Identity/Claims/Api/User/Delete"; 
             
        public const string AddDevice = "urn:Overlord/Identity/Claims/Api/Device/Create";        
        public const string UpdateDevice = "urn:Overlord/Identity/Claims/Api/Device/Update";
        public const string FindDevice = "urn:Overlord/Identity/Claims/Api/Device/Retrieve";
        public const string DeleteDevice = "urn:Overlord/Identity/Claims/Api/Device/Delete";

        public const string AddReading = "urn:Overlord/Identity/Claims/Api/Reading/Create";
    }
}
