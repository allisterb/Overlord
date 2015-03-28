﻿using System;
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
        private const string DeviceReadingKeyFormat = "{0}_{1:X5}";        
        public static DynamicTableEntity CreateSensorReadingEntity(IStorageDeviceReading reading)
        {
            Dictionary<string, EntityProperty> dictionary = new Dictionary<string, EntityProperty>();
            foreach (KeyValuePair<string, object> r in reading.SensorValues)
            {
                if (r.Key.ToSensorType() == typeof(string))
                {
                    dictionary.Add(r.Key, new EntityProperty((string)r.Value));
                    continue;
                }
                if (r.Key.ToSensorType() == typeof(int))
                {
                    dictionary.Add(r.Key, new EntityProperty((int)r.Value));
                    continue;
                }
                if (r.Key.ToSensorType() == typeof(double))
                {
                    dictionary.Add(r.Key, new EntityProperty((double)r.Value));
                    continue;
                }
                if (r.Key.ToSensorType() == typeof(DateTime))
                {
                    dictionary.Add(r.Key, new EntityProperty((DateTime)r.Value));
                    continue;
                }
                if (r.Key.ToSensorType() == typeof(bool))
                {
                    dictionary.Add(r.Key, new EntityProperty((bool)r.Value));
                    continue;
                }
                if (r.Key.ToSensorType() == typeof(byte[]))
                {
                    dictionary.Add(r.Key, new EntityProperty((byte[])r.Value));
                    continue;
                }
            }
            return new DynamicTableEntity(reading.Time.GeneratePartitionKey(),
                string.Format(CultureInfo.InvariantCulture, DeviceReadingKeyFormat, reading.DeviceId.ToUrn(),
                    reading.Time.GetTicks()), null, dictionary);                        
        }

        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Device)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.AddDeviceReading)]
        public IStorageDeviceReading AddDeviceReading(DateTime time, IDictionary<string, object> values)
        {
            if (values.Any(v => !v.Key.IsVaildSensorName())) 
            {
                string bad_sensors = values.Where(v => !v.Key.IsVaildSensorName())
                    .Select(v => v.Key + ":" + v.Value).Aggregate((a, b) => { return a + " " + b + ","; });
                throw new ArgumentException("Device reading has bad sensor names. {0}", bad_sensors);
            }

            if (values.Any(v => v.Key.ToSensorType() != v.Value.GetType().UnderlyingSystemType))
            {
                string bad_sensors = values.Where(v => v.Key.ToSensorType() != 
                    v.Value.GetType().UnderlyingSystemType)
                   .Select(v => v.Key + ":" + v.Value).Aggregate((a, b) => { return a + " " + b +","; });
                throw new ArgumentException(string.Format("Device reading has bad sensor values: {0}", 
                    bad_sensors));
            }
                
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindDevice);
            IStorageDevice device = this.GetCurrentDevice();
            IStorageDeviceReading reading = new IStorageDeviceReading()
            {
                DeviceId = device.Id,
                Time = time,
                SensorValues = values
            };            
            TableOperation insert_operation = TableOperation
                .InsertOrMerge(AzureStorage.CreateSensorReadingEntity(reading));
            TableResult result;
            try
            {
                result = this.SensorReadingsTable.Execute(insert_operation);
                reading.ETag = result.Etag;
                
                Log.WriteTableSuccess(string.Format
                    ("Added device reading entity: Partition: {0}, RowKey: {1}, Sensor values: {2}",                       
                       reading.Time.GeneratePartitionKey(),
                       string.Format(CultureInfo.InvariantCulture, DeviceReadingKeyFormat,
                             reading.DeviceId, reading.Time.GetTicks()),
                       reading.SensorValues                    
                           .Select(v => v.Key + ":" + v.Value)
                           .Aggregate((a, b) => { return a + "," + b + " "; })));
                
                return reading;                
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format
                    ("Added device reading entity: Partition: {0}, RowKey: {1}, {2}",
                       reading.Time.GeneratePartitionKey(),
                       string.Format(CultureInfo.InvariantCulture, DeviceReadingKeyFormat,
                           reading.DeviceId, reading.Time.GetTicks()),
                       reading.SensorValues
                           .Select(v => v.Key + ":" + v.Value)
                           .Aggregate((a, b) => { return a + "," + b + " "; })), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.AddDeviceReading);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Device)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.GetDeviceReading)]
        public SortedList<IStorageDeviceReading, IComparer<DateTime>> GetSensorReadings(string name, DateTime start, DateTime end)
        {
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindDevice);
            IStorageDevice device = this.GetCurrentDevice();
            DateTime start_minute =  start.AddMinutes(-1D);
            DateTime end_minute = start.AddMinutes(-1D);
            string start_pk = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0)
            .GeneratePartitionKey();
            string end_pk = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, 0)
            .GeneratePartitionKey();
            //TableOperation retrieve_operation = TableOperation
            //    .Retrieve<IStorageSensorReading>(start_pk, )
            //TimeSpan range = end - start;
            //range.
            return new SortedList<IStorageDeviceReading, IComparer<DateTime>> ();
        }
    }        
}
