using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Permissions;
using System.IdentityModel.Services;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

using Newtonsoft.Json;

using Overlord.Security;

using Overlord.Security.ClaimTypes;
using Overlord.Security.Claims;
using Overlord.Storage.Common;


namespace Overlord.Storage
{
    public partial class AzureStorage
    {        
        private const string SensorReadingKeyFormat = "{0}";
        private const string SensorReadingColumnFormat = "{0}_{1:X5}";

        public static DynamicTableEntity CreateSensorReadingEntity(IStorageSensorReading reading)
        {
            Dictionary<string, EntityProperty> dictionary = new Dictionary<string, EntityProperty>();
            //dictionary.Add("DeviceId", new EntityProperty(reading.DeviceId.ToUrn()));
            //dictionary.Add("Time", new EntityProperty(reading.Time));
            //dictionary.Add("SensorName", new EntityProperty(reading.SensorName));
            string name = string.Format(CultureInfo.InvariantCulture, SensorReadingColumnFormat,
                reading.SensorName, reading.Time.GetTicks());
            if (reading.SensorName.ToSensorType() == typeof(string))
            {
                
                dictionary.Add(name,new EntityProperty((string)reading.Reading));
            }
            if (reading.SensorName.ToSensorType() == typeof(int)) dictionary.Add(name, 
                new EntityProperty((int)reading.Reading));
            if (reading.SensorName.ToSensorType() == typeof(double)) dictionary.Add(name, 
                new EntityProperty((double)reading.Reading));
            if (reading.SensorName.ToSensorType() == typeof(DateTime)) dictionary.Add(name, 
                new EntityProperty((DateTime)reading.Reading));
            if (reading.SensorName.ToSensorType() == typeof(bool)) dictionary.Add(name, 
                new EntityProperty((bool)reading.Reading));
            if (reading.SensorName.ToSensorType() == typeof(byte[])) dictionary.Add(name, 
                new EntityProperty((byte[])reading.Reading));
            return new DynamicTableEntity(reading.Time.GeneratePartitionKey(), 
                string.Format(CultureInfo.InvariantCulture, SensorReadingKeyFormat, reading.DeviceId.ToUrn(),
                    reading.SensorName), null, dictionary);            
        }

        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Device)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.AddSensorReading)]
        public IStorageSensorReading AddSensorReading(string sensor_name, DateTime time, object val)
        {
            if (val.GetType() != sensor_name.ToSensorType()) throw
                    new ArgumentException("Sensor reading is not of type string.");
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindDevice);
            IStorageDevice device = this.FindDevice();
            if (!device.Sensors.Keys.Contains(sensor_name)) throw new
                ArgumentException(string.Format("Sensor {0} not found on device.", sensor_name));
            IStorageSensorReading reading = new IStorageSensorReading()
            {
                DeviceId = device.Id,
                SensorName = sensor_name,
                Time = time,
                Reading = val
            };
            
            TableOperation insert_operation = TableOperation
                .InsertOrMerge(AzureStorage.CreateSensorReadingEntity(reading));
            TableResult result;
            try
            {
                result = this.SensorReadingsTable.Execute(insert_operation);
                    Log.WriteTableSuccess(string.Format
                       ("Added sensor reading entity: Sensor Name: {0}, Partition: {1}, RowKey: {2}, " +
                        "Type: {3}, Value: {4} ", reading.SensorName, reading.Time.GeneratePartitionKey(), 
                         string.Format(CultureInfo.InvariantCulture, SensorReadingKeyFormat,
                             reading.DeviceId.ToUrn() + ":", reading.SensorName),                            
                             reading.SensorName.ToSensorType().ToString(), reading.Reading));
                reading.ETag = result.Etag;
                return reading;                
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format
                   ("Failed to add sensor reading entity: Sensor Name: {0}, Partition: {1}, RowKey: {2}, " +
                        "Type: {3}, Value: {4} ", reading.SensorName, reading.Time.GeneratePartitionKey(), 
                         string.Format(CultureInfo.InvariantCulture, SensorReadingKeyFormat,
                             reading.DeviceId.ToUrn(),
                             reading.SensorName,
                             reading.Time.GetTicks()), 
                             reading.SensorName.ToSensorType().ToString() , 
                             reading.Reading), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.AddSensorReading);
            }
        }
    }        
}
