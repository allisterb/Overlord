﻿using System;
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
    public class AzureStorageDeviceReadingTests
    {      
        [Fact]
        public void CanParseSensorReadingType()
        {
            Assert.True("S1".ToSensorType() == typeof(string));
            Assert.True(TestData.sensor_01_reading_01.GetType() == 
                TestData.sensor_01_name.ToSensorType());
            Assert.False(TestData.sensor_03_reading_01.GetType() == TestData.sensor_01_name.ToSensorType());
            Assert.True(TestData.sensor_02_reading_01.GetType() ==
                TestData.sensor_02_name.ToSensorType());
            Assert.False(TestData.sensor_03_reading_01.GetType() == TestData.sensor_02_name.ToSensorType());

                       
        }
        
        [Fact]
        public void CanAddDeviceReading()
        {
            OverlordIdentity.InitializeAnonymousIdentity();
            AzureStorage storage = new AzureStorage();            
            storage.AuthenticateAnonymousDevice(TestData.device_01_id.UrnToId(), 
                TestData.device_01_token);            
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddSensor);
            storage.AddSensor(TestData.sensor_01_name, TestData.sensor_01_unit, null, null);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDeviceReading);
            Dictionary<string, object> sensor_values = new Dictionary<string,object>() 
            {
                {TestData.sensor_01_name, TestData.sensor_01_reading_01} 
            };
            storage.AddDeviceReading(DateTime.Now, sensor_values);            
        }
    }
}
