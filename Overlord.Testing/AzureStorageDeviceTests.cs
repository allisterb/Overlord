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
    public class AzureStorageDeviceTests
    {

        private void InitialiseTestData()
        {
            AzureStorage storage = new AzureStorage();            
            OverlordIdentity.InitializeUserIdentity(TestData.user_02_id, TestData.user_02_token, new string[0]);
            
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            IStorageUser user = storage.FindUser(TestData.user_02_id.UrnToGuid(), TestData.user_02_token);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDevice);
            IStorageDevice device_01 = storage.AddDevice(user, TestData.device_01_name, TestData.device_01_token, null, TestData.device_01_id);

            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            //user = storage.FindUser(AzureStorageTests.user_02_id.UrnToGuid(), AzureStorageTests.user_02_token);            
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDevice);
            IStorageDevice device_02 = storage.AddDevice(user, TestData.device_02_name, TestData.device_02_token, null, TestData.device_02_id);
            
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            //user = storage.FindUser(AzureStorageTests.user_02_id.UrnToGuid(), AzureStorageTests.user_02_token);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDevice);
            IStorageDevice device_03 = storage.AddDevice(user, TestData.device_03_name, TestData.device_03_token, null, TestData.device_03_id);
        }
        
        [Fact]
        public void CanParseUrns()
        {
            Guid g = TestData.device_01_id.UrnToGuid();
            Assert.Equal(Guid.ParseExact("9ac31883-f0e3-4666-a05f-6add31beb8f4", "D"), g);
            Assert.Equal("urn:uuid:9ac31883-f0e3-4666-a05f-6add31beb8f4".UrnToGuid(), 
                Guid.ParseExact("9ac31883-f0e3-4666-a05f-6add31beb8f4", "D"));
            Assert.Equal(TestData.device_01_id.UrnToId(), g.ToString("D"));
        }

                        
        [Fact]
        public void CanAddDevice()
        {                        
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.InitializeUserIdentity(TestData.user_02_id, 
                TestData.user_02_token, new string[0]);  
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            IStorageUser user = storage.FindUser(TestData.user_02_id.UrnToGuid(), 
                TestData.user_02_token);
            Assert.NotNull(user);        
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDevice);
            IStorageDevice device = storage.AddDevice(user, "xUnit_CanAddDeviceTest_Name",
                "xUnit_CanAddDeviceTest_Token", null);
            Assert.NotNull(device.Id);
            Assert.Equal("xUnit_CanAddDeviceTest_Name", device.Name);
            Assert.Equal("xUnit_CanAddDeviceTest_Token", device.Token);
            Assert.True(user.Devices.Contains(device.Id));                        
        }


        [Fact]
        public void CanFindDevice()
        {
            //InitialiseTestData();
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.InitializeUserIdentity(TestData.user_02_id, TestData.user_02_token, new string[0]);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            IStorageUser user = storage.FindUser(TestData.user_01_id.UrnToGuid(), TestData.user_01_token);
            Assert.NotNull(user);          
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindDevice);
            IStorageDevice device = storage.FindDevice(TestData.device_01_id.UrnToGuid(), TestData.device_01_token);            
            Assert.NotNull(device);
        }

        
        
    }
}
