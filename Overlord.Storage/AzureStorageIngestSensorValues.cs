using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Permissions;
using System.IdentityModel.Services;
using System.Text;
using System.Threading;
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
        public static DynamicTableEntity CreateSensorValueEntity(DateTime time, string name, object value)
        {
            Dictionary<string, EntityProperty> dictionary = new Dictionary<string, EntityProperty>();
            if (name.ToSensorType() == typeof(string))
            {
                dictionary.Add(name + "_" + time.GetTicks(), new EntityProperty((string)value));
            }
            if (name.ToSensorType() == typeof(int))
            {
                if (value.GetType() == typeof(Int64))
                {
                    dictionary.Add(name + "_" + time.GetTicks(), new EntityProperty((Int64)value));
                }
                else
                {
                    dictionary.Add(name + "_" + time.GetTicks(), new EntityProperty((Int32)value));
                }
            }
            if (name.ToSensorType() == typeof(double))
            {
                dictionary.Add(name + "_" + time.GetTicks(), new EntityProperty((double)value));
            }
            if (name.ToSensorType() == typeof(DateTime))
            {
                dictionary.Add(name + "_" + time.GetTicks(), new EntityProperty((DateTime)value));
            }
            if (name.ToSensorType() == typeof(bool))
            {
                dictionary.Add(name + "_" + time.GetTicks(), new EntityProperty((bool)value));
            }
            if (name.ToSensorType() == typeof(byte[]))
            {
                dictionary.Add(name + "_" + time.GetTicks(), new EntityProperty((byte[])value));
            }
            return new DynamicTableEntity(time.GeneratePartitionKey(), name, "*", dictionary);
        }

        public void IngestSensorValues(IStorageDeviceReading reading)
        {
            
            CloudTable device_channel = this.TableClient.GetTableReference(reading.DeviceId.ToTableName());
            device_channel.CreateIfNotExists();
            Parallel.ForEach(reading.SensorValues, sv =>
            {
                TableOperation insert_operation = TableOperation
                    .InsertOrMerge(AzureStorage.CreateSensorValueEntity(reading.Time, sv.Key, sv.Value));
                TableResult result = device_channel.Execute(insert_operation);
            });

        }
    }
}
