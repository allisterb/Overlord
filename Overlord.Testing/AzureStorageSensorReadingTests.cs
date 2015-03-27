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
            Assert.True(TestData.sensor_01_reading_01.GetType() == "S1".ToSensorType());
            Assert.False(TestData.sensor_03_reading_01.GetType() == "S1".ToSensorType());
                       
        }
        
        [Fact]
        public void CanAddSensorReading()
        {
            OverlordIdentity.InitializeAnonymousIdentity();
            AzureStorage storage = new AzureStorage();            
            storage.AuthenticateAnonymousDevice(TestData.device_01_id.UrnToId(), 
                TestData.device_01_token);            
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddSensor);
            storage.AddSensor(TestData.sensor_01_name, TestData.sensor_01_unit, null, null);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddSensorReading);
            storage.AddSensorReading(TestData.sensor_01_name, DateTime.Now, TestData.sensor_01_reading_01);
        }
    }
}
