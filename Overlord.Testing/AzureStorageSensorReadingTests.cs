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
    public class AzureStorageSensorReadingTests
    {      
        [Fact]
        public void CanParseSensorReadingType()
        {
            Assert.True("S1".ToSensorType() == typeof(string));
        }

        [Fact]
        public void CanAddSensorReading()
        {
            AzureStorage storage = new AzureStorage();            
            OverlordIdentity.InitializeDeviceIdentity(AzureStorageTests.device_01_id, 
                AzureStorageTests.device_01_token, null);            
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddSensor);
            storage.AddSensor("S1", "", null, null);
            storage.AddSensorReading("S1", DateTime.Now, 32);
        }
    }
}
