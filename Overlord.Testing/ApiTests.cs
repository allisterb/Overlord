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
    public class ApiTests
    {
        public const string user_01_id = "urn:uuid:d155074f-4e85-4cb5-a597-8bfecb0dfc04";
        public const string user_01_token = "admin";
        public const string user_02_id = "urn:uuid:db4d2be1-a495-4ee0-9f49-03aa72a93f6b";
        public const string user_03_id = "urn:uuid:fba0c871-41d6-4ae1-83d6-f024a84adeb4";

        public const string device_01_id = "urn:uuid:9ac31883-f0e3-4666-a05f-6add31beb8f4";
        public const string device_01_token = "XUnit_CanFindDevice_Test_Token";


        [Fact]
        public void CanParseUrns()
        {
            Guid g = device_01_id.UrnToGuid();
            Assert.Equal(Guid.ParseExact("9ac31883-f0e3-4666-a05f-6add31beb8f4", "D"), g);
            Assert.Equal("urn:uuid:9ac31883-f0e3-4666-a05f-6add31beb8f4".UrnToGuid(), Guid.ParseExact("9ac31883-f0e3-4666-a05f-6add31beb8f4", "D"));
            Assert.Equal(device_01_id.UrnToId(), g.ToString("D"));
        }

        [Fact]
        public void CanInitializeIdentity()
        {
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Anonymous));
            OverlordIdentity.InitializeAnonymousIdentity();
            Assert.True(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Anonymous));
            OverlordIdentity.InitializeDeviceIdentity(device_01_id.UrnToId(), device_01_token, new string[0]);
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Anonymous));
            Assert.True(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Device));
            OverlordIdentity.InitializeUserIdentity(user_01_id.UrnToId(), user_01_token, new string[0]);
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Device));
            Assert.True(OverlordIdentity.HasClaim(Authentication.Role, UserRole.User));
        }
    }

}
