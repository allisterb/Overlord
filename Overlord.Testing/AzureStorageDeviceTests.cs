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
    public class AzureDeviceStorage
    {
        public const string user_01_id = "urn:uuid:d155074f-4e85-4cb5-a597-8bfecb0dfc04";
        public const string user_02_id = "urn:uuid:db4d2be1-a495-4ee0-9f49-03aa72a93f6b";
        public const string user_03_id = "urn:uuid:fba0c871-41d6-4ae1-83d6-f024a84adeb4";

        public const string device_01_id = "urn:uuid:9ac31883-f0e3-4666-a05f-6add31beb8f4";
        public const string device_01_token = "XUnit_CanFindDevice_Test_Token";
        public const string device_02_id = "urn:uuid:618e7ae4-7ea8-4405-b229-e2fca57fd817";
        public const string device_03_id = "urn:uuid:84512c8b-b531-4067-8f42-e15656edb0a3";
        public const string device_04_id = "urn:uuid:9094b12e-e92c-44fc-bf64-54628e896e20";
        public const string device_05_id = "urn:uuid:3753de29-d028-4b0b-b4f4-2e4d8e31538b";

        public const string sensor_01_name = "CanAddSensor Test";
        public const string sensor_01_unit = "Degrees";

        public void CanParseUrns()
        {
            Guid g = device_01_id.UrnToGuid();
            Assert.Equal(Guid.ParseExact("9ac31883-f0e3-4666-a05f-6add31beb8f4", "D"), g);
            Assert.Equal("urn:uuid:9ac31883-f0e3-4666-a05f-6add31beb8f4".UrnToGuid(), Guid.ParseExact("9ac31883-f0e3-4666-a05f-6add31beb8f4", "D"));
            Assert.Equal(user_01_id.UrnToId(), g.ToString("D"));
        }

        [Fact]
        public void CanAddDevice()
        {                        
            AzureStorage storage = new AzureStorage();
            //OverlordIdentity.InitalizeIdentity();  
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            IStorageUser user = storage.FindUser(user_01_id.UrnToGuid(), "admin");
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDevice);
            IStorageDevice device = storage.AddDevice(user, "XUnit_CanAddDevice_Test_Name", "XUnit_CanAddDevice_Test_Token", null);
            Assert.NotNull(device.Id);
            Assert.Equal("XUnit_CanAddDevice_Test_Name", device.Name);
            Assert.Equal("XUnit_CanAddDevice_Test_Token", device.Token);
        }


        [Fact]
        public void CanFindDevice()
        {
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.InitializeUserIdentity(user_01_id, "admin", new string[0]);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            IStorageUser user = storage.FindUser(user_01_id.UrnToGuid(), "admin");
            Assert.NotNull(user);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindDevice);
            IStorageDevice device = storage.FindDevice(device_01_id.UrnToGuid(), "XUnit_CanFindDevice_Test_Token");            
            Assert.NotNull(device);
            Assert.Contains(device, user.Devices, new IStorageDeviceEq());
            Assert.True(user.Devices.ContainsDevice(device));                                
        }

        [Fact]
        public void CanAddSensor()
        {
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.InitializeDeviceIdentity(device_01_id.UrnToId(), device_01_token, new string[0]);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddSensor);
            IStorageSensor sensor = storage.AddSensor(sensor_01_name, sensor_01_unit, null, null);
            Assert.NotNull(sensor);
            Assert.Equal(sensor.Name, sensor_01_name);
            Assert.Equal(sensor.Unit, sensor_01_unit);
        }
        
    }
}
