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
                
        [Fact]
        public void CanParseUrns()
        {
            Guid g = AzureStorageTests.user_01_id.UrnToGuid();
            Assert.Equal(Guid.ParseExact("d155074f-4e85-4cb5-a597-8bfecb0dfc04", "D"), g);
            Assert.Equal("urn:uuid:d155074f-4e85-4cb5-a597-8bfecb0dfc04".UrnToGuid(), 
                Guid.ParseExact("d155074f-4e85-4cb5-a597-8bfecb0dfc04", "D"));
        }

        [Fact]
        public void CanAuthorizeAuthenticateAnonymousUser()
        {
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.InitializeUserIdentity(AzureStorageTests.user_01_id.UrnToId(), "admin", new string[0]);
            Assert.Throws(typeof(System.Security.SecurityException), 
                () => storage.AuthenticateAnonymousUser(AzureStorageTests.user_01_id, "admin"));
        }

        [Fact]
        public void CanAuthenticateAnonymousUser()
        {
            OverlordIdentity.InitializeAnonymousIdentity();
            AzureStorage storage = new AzureStorage();
            Assert.False(storage.AuthenticateAnonymousUser(AzureStorageTests.user_01_id.UrnToId(), "foo"));
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.User));
            Assert.True(storage.AuthenticateAnonymousUser(AzureStorageTests.user_01_id.UrnToId(), 
                AzureStorageTests.user_01_token));
            Assert.True(OverlordIdentity.HasClaim(Authentication.Role, UserRole.User));
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Anonymous));
        }

        [Fact]
        public void CanAuthorizeAddUser()
        {
            OverlordIdentity.InitializeUserIdentity(AzureStorageTests.user_01_id.UrnToId(), "admin", new string[0]);
            AzureStorage storage = new AzureStorage();
            Assert.Throws(typeof(System.Security.SecurityException), () => 
                storage.AddUser("XUnit_CanAuthorizeAddAzureStorageTests.user_Test_Name", "XUnit_CanAuthorizeAddAzureStorageTests.user_Test_Token", 
                null));
        }

        [Fact]
        public void CanAuthorizeDeleteUser()
        {
            OverlordIdentity.InitializeUserIdentity(AzureStorageTests.user_01_id.UrnToId(), "admin", new string[0]);            
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            IStorageUser user = storage.FindUser("d155074f-4e85-4cb5-a597-8bfecb0dfc04".ToGuid(), "admin");
            Assert.Throws(typeof(System.Security.SecurityException), () => storage.DeleteUser(user));
            OverlordIdentity.InitializeAdminUserIdentity(AzureStorageTests.user_01_id.UrnToId(), "admin", new string[0]);                        
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
            Assert.True(storage.AuthenticateAnonymousDevice(AzureStorageTests.device_01_id.UrnToId(),
                AzureStorageTests.device_01_token));
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
            OverlordIdentity.InitializeDeviceIdentity(AzureStorageTests.device_01_id.UrnToId(), AzureStorageTests.device_01_token, new string[0]);
            Assert.Throws(typeof(System.Security.SecurityException), () =>
                storage.AddSensor("foo", "bar", null, null));            
            
            //Doesn't throw security exception when proper permission is present.
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddSensor);
            IStorageSensor s = storage.AddSensor(AzureStorageTests.sensor_01_name, "bar", null, null);
            Assert.NotNull(s);
            Assert.True(s.Name == AzureStorageTests.sensor_01_name);
        }

    }
}
