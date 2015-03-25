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
    public class AzureStorageUserTests
    {
        private void InitialiseTestData()
        {
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.InitializeAdminUserIdentity(AzureStorageTests.user_01_id, AzureStorageTests.user_01_token, new string[0]);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddUser);
            IStorageUser user_01 = storage.AddUser(AzureStorageTests.user_01_name, AzureStorageTests.user_01_token, null, AzureStorageTests.user_01_id);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddUser);
            IStorageUser user_02 = storage.AddUser(AzureStorageTests.user_02_name, AzureStorageTests.user_02_token, null, AzureStorageTests.user_02_id);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddUser);
            IStorageUser user_03 = storage.AddUser(AzureStorageTests.user_03_name, AzureStorageTests.user_03_token, null, AzureStorageTests.user_03_id);            
        }

        [Fact]
        public void CanAddUser()
        {
            OverlordIdentity.InitializeAdminUserIdentity(AzureStorageTests.user_01_id, 
                AzureStorageTests.user_01_token, new string[0]);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddUser);
            AzureStorage storage = new AzureStorage();
            IStorageUser user = storage.AddUser("xUnit_CanAddUserTest_Name", "xUnit_CanAddUserTest_Token", null);
            Assert.NotNull(user.Id);
            Assert.Equal("xUnit_CanAddUserTest_Name", user.UserName);
            Assert.Equal("xUnit_CanAddUserTest_Token", user.Token);
        }

        [Fact]
        public void CanFindUser()
        {
            //InitialiseTestData();
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            IStorageUser user = storage.FindUser(AzureStorageTests.user_02_id.UrnToGuid(), AzureStorageTests.user_02_token);
            Assert.NotNull(user);
            Assert.Equal(user.UserName, AzureStorageTests.user_02_name);
            Assert.Equal(user.Token, AzureStorageTests.user_02_token);
        } 
    }
}
