using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

using Xunit;

using Overlord.Security;
using Overlord.Security.Claims;
using Overlord.Security.ClaimTypes;
using Overlord.Storage;

namespace Overlord.Testing
{
    public class AzureStorageDeviceReadingTests
    {
        private ObservableEventListener storage_event_log_listener = new ObservableEventListener();
        private AzureStorageEventSource Log = AzureStorageEventSource.Log;

        public AzureStorageDeviceReadingTests()
        {
            EventTextFormatter formatter = new EventTextFormatter() { VerbosityThreshold = EventLevel.Error };
            storage_event_log_listener.EnableEvents(AzureStorageEventSource.Log, EventLevel.LogAlways,
                AzureStorageEventSource.Keywords.Perf | AzureStorageEventSource.Keywords.Diagnostic);
            storage_event_log_listener.LogToFlatFile("Overlord.Storage.Azure.log", formatter, true);          
        }

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
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindDevice);
            IStorageDevice device = storage.GetCurrentDevice();
            IDictionary<string, object> sensor_values = TestData.GenerateRandomSensorData(10);
            foreach (KeyValuePair<string, object> s in sensor_values)
            {
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddSensor);
                storage.AddSensor(s.Key, "CanAddDeviceReading_Test", null, null);                
            }
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDeviceReading);
            storage.AddDeviceReading(DateTime.Now, sensor_values);                                    
        }

        [Fact]
        public void CanAddDeviceReadingParallel()
        {
            Dictionary<int, IStorageDevice> devices = new Dictionary<int,IStorageDevice>();
            OverlordIdentity.InitializeAnonymousIdentity();
            AzureStorage storage = new AzureStorage();
            storage.AuthenticateAnonymousUser(TestData.user_02_id.UrnToId(),
                TestData.user_02_token);
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);                
            IStorageUser user = storage.GetCurrentUser();
            for (int i = 0; i <= 5; i++)

            {                
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDevice);                
                IStorageDevice device = storage.AddDevice(user, "xUnit_CanAddDevicereadingParallel_Device_" + i.ToString(),
                    "xUnit_CanAddDevicereadingParallel_Device_Token", null, null);                                                    
                devices.Add(i, device);
            }
            OverlordIdentity.InitializeAnonymousIdentity();            
            Parallel.For(0, devices.Count, d =>
            {
                Log.Partition();
                storage.AuthenticateAnonymousDevice(devices[d].Id.ToUrn(), devices[0].Token);
                IDictionary<string, object> sensor_values = TestData.GenerateRandomSensorData(10);                
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDeviceReading);
                storage.AddDeviceReading(TestData.GenerateRandomTime(null, null, null, null),
                        sensor_values);
                //Sleep for a random interval
                Thread.Sleep(TestData.GenerateRandomInteger(0, 1000));
              
                //Add another set of sensor data
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDeviceReading);
                storage.AddDeviceReading(TestData.GenerateRandomTime(null, null, null, null),
                        TestData.GenerateRandomSensorData(sensor_values));
                
                //Sleep for a random interval
                Thread.Sleep(TestData.GenerateRandomInteger(0, 1000));

                //Add another set of sensor data
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDeviceReading);
                storage.AddDeviceReading(TestData.GenerateRandomTime(null, null, null, null),
                        TestData.GenerateRandomSensorData(sensor_values));
            });
        }
        
    }
}
