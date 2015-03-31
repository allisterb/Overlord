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
    public class AzureStorageIngestTests
    {
        private ObservableEventListener storage_event_log_listener = new ObservableEventListener();
        private AzureStorageEventSource Log = AzureStorageEventSource.Log;

        public AzureStorageIngestTests()
        {
            EventTextFormatter formatter = new EventTextFormatter() { VerbosityThreshold = EventLevel.Error };
            storage_event_log_listener.EnableEvents(AzureStorageEventSource.Log, EventLevel.LogAlways,
                AzureStorageEventSource.Keywords.Perf | AzureStorageEventSource.Keywords.Diagnostic);
            storage_event_log_listener.LogToFlatFile("Overlord.Storage.Azure.log", formatter, true);          
        }
        
        private void CreateDeviceReadings()
        {
            IList<IStorageChannel> channels = CreateSensorChannels();
            Dictionary<int, IStorageDevice> devices = new Dictionary<int, IStorageDevice>();
            OverlordIdentity.InitializeAnonymousIdentity();
            AzureStorage storage = new AzureStorage();
            storage.AuthenticateAnonymousUser(TestData.user_02_id.UrnToId(),
                TestData.user_02_token);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            IStorageUser user = storage.GetCurrentUser();
            for (int i = 0; i <= 5; i++)
            {
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDevice);
                IStorageDevice device = storage.AddDevice(user, "xUnit_IngestTests_Device_" + i.ToString(),
                    "xUnit_IngestTests_Device_Token", null, null);
                devices.Add(i, device);
            }
            IDictionary<int, IDictionary<string, object>> sensor_values = new Dictionary<int, IDictionary<string, object>>(6);
            for (int d = 0; d <= 5; d++)
            {
                sensor_values[d] = TestData.GenerateRandomSensorData(10);
                foreach (string s in sensor_values[d].Keys)
                {
                    IStorageSensor sensor = new IStorageSensor()
                    {                        
                        DeviceId = devices[d].Id,
                        Name = s,
                        Channels = new List<Guid>()
                    };
                    List<IStorageAlert> alerts = new List<IStorageAlert>();
                    foreach (IStorageChannel channel in channels.Where(c => c.SensorType.ToSensorType() ==
                        sensor.Name.ToSensorType()))
                    {
                        sensor.Channels.Add(channel.Id);
                    }
                    OverlordIdentity.AddClaim(Resource.Storage, StorageAction.UpdateDevice);
                    devices[d].Sensors.Add(new KeyValuePair<string, IStorageSensor>(s, sensor));
                    devices[d] = storage.UpdateDevice(devices[d]);
                }

            }
             OverlordIdentity.InitializeAnonymousIdentity();
             storage.AuthenticateAnonymousDevice(devices[0].Id.ToUrn(), devices[0].Token);
            Parallel.For(0, devices.Count, d =>
            {
                Log.Partition();                
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDeviceReading);
                storage.AddDeviceReading(devices[d], TestData.GenerateRandomTime(null, null, null, null),
                        sensor_values[d]);
                //Sleep for a random interval
                Thread.Sleep(TestData.GenerateRandomInteger(0, 1000));

                //Add another set of sensor data
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDeviceReading);
                storage.AddDeviceReading(devices[d], TestData.GenerateRandomTime(null, null, null, null),
                        TestData.GenerateRandomSensorData(sensor_values[d]));

                //Sleep for a random interval
                Thread.Sleep(TestData.GenerateRandomInteger(0, 1000));

                //Add another set of sensor data
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDeviceReading);
                storage.AddDeviceReading(devices[d], TestData.GenerateRandomTime(null, null, null, null),
                        TestData.GenerateRandomSensorData(sensor_values[d]));
            });
        }

        private IList<IStorageChannel> CreateSensorChannels()
        {            
            OverlordIdentity.InitializeAnonymousIdentity();
            AzureStorage storage = new AzureStorage();
            storage.AuthenticateAnonymousDevice(TestData.device_02_id.UrnToId(),
                TestData.device_02_token);
            //OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindDevice);
            //IStorageDevice = storage.GetCurrentUser();            
            
            IStorageAlert alert_1 = new IStorageAlert() 
            {
                SensorType = "I0",
                IntMaxValue = 100,
                IntMinValue = -10
            };
            IStorageAlert alert_2 = new IStorageAlert() 
            {
                SensorType = "I0",
                IntMaxValue = 40,
                IntMinValue = 0
            };

            List<IStorageAlert> alerts = new List<IStorageAlert>() {alert_1, alert_2};
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddChannel);
            IStorageChannel channel = storage.AddChannel("xUnit_IngestTests_Channel_01",
                    "A test channel for xUnit AzureStorageIngestTests", "I0", "Test integer units", 
                    alerts);
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddChannel);
            alert_1 = new IStorageAlert()
            {
                SensorType = "S0",
                StringValue = "Alert 1!"
            };
            alert_2 = new IStorageAlert()
            {
                SensorType = "S0",
                StringValue = "Alert 1!"
            };

            alerts = new List<IStorageAlert>() { alert_1, alert_2 };
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddChannel);
            IStorageChannel channel_2 = storage.AddChannel("xUnit_IngestTests_Channel_03",
                    "A test channel for xUnit AzureStorageIngestTests", "S0", "Test stringr units",
                    alerts);

            return new List<IStorageChannel>() { channel, channel_2 };
        }

        [Fact]
        public void CanIngestSensorValues ()
        {            
            CreateDeviceReadings();
            AzureStorage storage = new AzureStorage();
            storage.IngestSensorValues();
        }
    }
}
