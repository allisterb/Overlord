using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Overlord.Security;
using Overlord.Security.Claims;
using Overlord.Security.ClaimTypes;
using Overlord.Storage;
using Xunit;

namespace Overlord.Testing
{
    public class AzureStorageSensorTests
    {
        [Fact]
        public void CanParseSensorNames()
        {
            Assert.False("4xf".IsVaildSensorName());
            Assert.False("46577575".IsVaildSensorName());
            Assert.False("X4".IsVaildSensorName());
            Assert.True("D4".IsVaildSensorName());
            Assert.True("D4L1".IsVaildSensorName());
            Assert.True("N345".IsVaildSensorName());
        }
        
        [Fact]
        public void CanAddSensor()
        {
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.InitializeAnonymousIdentity();
            storage.AuthenticateAnonymousDevice(TestData.device_02_id.UrnToId(), 
                TestData.device_02_token);            
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddSensor);
            IStorageSensor sensor = storage.AddSensor(TestData.sensor_01_name, TestData.sensor_01_unit, null, null);
            Assert.NotNull(sensor);
            Assert.Equal(sensor.Name, TestData.sensor_01_name);
            Assert.Equal(sensor.Unit, TestData.sensor_01_unit);
        }
    }
}
