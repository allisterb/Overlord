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
    public class SecurityTests
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

        public const string sensor_01_name = "xUnit_AddSensor_Test";

        [Fact]
        public void CanParseUrns()
        {
            Guid g = user_01_id.UrnToGuid();
            Assert.Equal(Guid.ParseExact("d155074f-4e85-4cb5-a597-8bfecb0dfc04", "D"), g);
            Assert.Equal("urn:uuid:d155074f-4e85-4cb5-a597-8bfecb0dfc04".UrnToGuid(), 
                Guid.ParseExact("d155074f-4e85-4cb5-a597-8bfecb0dfc04", "D"));
        }

        [Fact]
        public void CanAuthorizeAuthenticateAnonymousUser()
        {
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.InitializeUserIdentity(user_01_id.UrnToId(), "admin", new string[0]);
            Assert.Throws(typeof(System.Security.SecurityException), 
                () => storage.AuthenticateAnonymousUser(user_01_id, "admin"));
        }

        [Fact]
        public void CanAuthenticateAnonymousUser()
        {
            OverlordIdentity.InitializeAnonymousIdentity();
            AzureStorage storage = new AzureStorage();
            Assert.NotNull(storage.AuthenticateAnonymousUser(user_01_id.UrnToId(), "admin"));
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Anonymous));
            Assert.True(OverlordIdentity.HasClaim(Authentication.Role, UserRole.User));
        }

        [Fact]
        public void CanAuthorizeAddUser()
        {
            OverlordIdentity.InitializeUserIdentity(user_01_id.UrnToId(), "admin", new string[0]);
            AzureStorage storage = new AzureStorage();
            Assert.Throws(typeof(System.Security.SecurityException), () => 
                storage.AddUser("XUnit_CanAuthorizeAddUser_Test_Name", "XUnit_CanAuthorizeAddUser_Test_Token", 
                null));
        }

        [Fact]
        public void CanAuthorizeDeleteUser()
        {
            OverlordIdentity.InitializeUserIdentity(user_01_id.UrnToId(), "admin", new string[0]);
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            IStorageUser user = storage.FindUser("d155074f-4e85-4cb5-a597-8bfecb0dfc04".ToGuid(), "admin");
            Assert.Throws(typeof(System.Security.SecurityException), () => storage.DeleteUser(user));
        }

        [Fact]
        public void CanAuthorizeFindUser()
        {            
            AzureStorage storage = new AzureStorage();
            Assert.Throws(typeof(System.Security.SecurityException), () => 
                storage.FindUser("d155074f-4e85-4cb5-a597-8bfecb0dfc04".ToGuid(), "admin"));
        }

        [Fact]
        public void CanAuthenticateAnonymousDevice()
        {
            OverlordIdentity.InitializeAnonymousIdentity();                            
            AzureStorage storage = new AzureStorage();
            Assert.True(storage.AuthenticateAnonymousDevice(device_01_id.UrnToId(), 
                "XUnit_CanFindDevice_Test_Token"));
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Anonymous));
            Assert.True(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Device));
        }

        [Fact]
        public void CanAuthorizeAddSensor()
        {
            AzureStorage storage = new AzureStorage();
            Assert.Throws(typeof(System.Security.SecurityException), () =>
                storage.AddSensor("foo", "bar", null, null));
            
            //Throws security exception even if correct identity.
            OverlordIdentity.InitializeDeviceIdentity(device_01_id.UrnToId(), device_01_token, new string[0]);
            Assert.Throws(typeof(System.Security.SecurityException), () =>
                storage.AddSensor("foo", "bar", null, null));            
            
            //Doesn't throw security exception when proper permission is present.
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddSensor);
            IStorageSensor s = storage.AddSensor(sensor_01_name, "bar", null, null);
            Assert.NotNull(s);
            Assert.True(s.Name == sensor_01_name);
        }

    }
}
