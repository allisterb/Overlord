using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

using Xunit;

using Overlord.Core;
using Overlord.Security;
using Overlord.Security.Claims;
using Overlord.Security.ClaimTypes;
using Overlord.Storage;

namespace Overlord.Testing
{
    public class ApiTests
    {
        
        public ApiTests()
        {
            
        }

        [Fact]
        public void CanParseUrns()
        {
            Guid g = TestData.device_01_id.UrnToGuid();
            Assert.Equal(Guid.ParseExact("9ac31883-f0e3-4666-a05f-6add31beb8f4", "D"), g);
            Assert.Equal("urn:uuid:9ac31883-f0e3-4666-a05f-6add31beb8f4".UrnToGuid(), Guid.ParseExact("9ac31883-f0e3-4666-a05f-6add31beb8f4", "D"));
            Assert.Equal(TestData.device_01_id.UrnToId(), g.ToString("D"));
        }

        [Fact]
        public void CanInitializeIdentity()
        {
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Anonymous));
            OverlordIdentity.InitializeAnonymousIdentity();
            Assert.True(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Anonymous));
            OverlordIdentity.InitializeDeviceIdentity(TestData.device_01_id.UrnToId(), TestData.device_01_token, new string[0]);
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Anonymous));
            Assert.True(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Device));
            OverlordIdentity.InitializeUserIdentity(TestData.user_01_id.UrnToId(), TestData.user_01_token, new string[0]);
            Assert.False(OverlordIdentity.HasClaim(Authentication.Role, UserRole.Device));
            Assert.True(OverlordIdentity.HasClaim(Authentication.Role, UserRole.User));
        }

        [Fact]
        public void CanAddDeviceReading()
        {         
            Assert.True(Api.AuthenticateDevice(TestData.device_01_id, TestData.device_01_token));            
            OverlordIdentity.AddClaim(Resource.Api, ApiAction.AddReading);
            DateTime time = TestData.GenerateRandomTime(null, null, null, 
                    DateTime.Now.Hour);
            IDictionary<string, object> sensor_values = new Dictionary<string, object>()
            {
                {TestData.sensor_01_name, TestData.GenerateRandomString(43)}
            };
            Api.AddDeviceReading(time, sensor_values);
        }
    }

}
