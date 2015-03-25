using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using Overlord.Security;
using Overlord.Security.Claims;
using Overlord.Security.ClaimTypes;
using Overlord.Storage;


namespace Overlord.Testing
{
    internal static class AzureStorageTests
    {
        public const string user_01_id = "urn:uuid:d155074f-4e85-4cb5-a597-8bfecb0dfc04";
        public const string user_01_token = "xUnit_TestUser_01_Token";
        public const string user_01_name = "xUnit_TestUser_01_Name";

        public const string user_02_id = "urn:uuid:db4d2be1-a495-4ee0-9f49-03aa72a93f6b";
        public const string user_02_token = "xUnit_TestUser_02_Token";
        public const string user_02_name = "xUnit_TestUser_02_Name";

        public const string user_03_id = "urn:uuid:fba0c871-41d6-4ae1-83d6-f024a84adeb4";
        public const string user_03_token = "xUnit_TestUser_03_Token";
        public const string user_03_name = "xUnit_TestUser_03_Name";

        public const string device_01_id = "urn:uuid:9ac31883-f0e3-4666-a05f-6add31beb8f4";
        public const string device_01_token = "xUnit_TestDevice_01_Token";
        public const string device_01_name = "xUnit_TestDevice_01_Name";

        public const string device_02_id = "urn:uuid:618e7ae4-7ea8-4405-b229-e2fca57fd817";
        public const string device_02_token = "xUnit_TestDevice_02_Token";
        public const string device_02_name = "xUnit_TestDevice_02_Name";

        public const string device_03_id = "urn:uuid:84512c8b-b531-4067-8f42-e15656edb0a3";
        public const string device_03_token = "xUnit_TestDevice_03_Token";
        public const string device_03_name = "xUnit_TestDevice_03_Name";

        public const string device_04_id = "urn:uuid:9094b12e-e92c-44fc-bf64-54628e896e20";
        public const string device_04_token = "xUnit_TestDevice_04_Token";
        public const string device_04_name = "xUnit_TestDevice_04_Name";

        public const string device_05_id = "urn:uuid:3753de29-d028-4b0b-b4f4-2e4d8e31538b";
        public const string device_05_token = "xUnit_TestDevice_05_Token";
        public const string device_05_name = "xUnit_TestDevice_05_Name";

        public const string sensor_01_name = "CanAddSensor Test";
        public const string sensor_01_unit = "Degrees";
                               
    }
}
