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

        public static DynamicTableEntity CreateChannelItemEntity(DateTime time, string name, object value)
        {
            Dictionary<string, EntityProperty> dictionary = new Dictionary<string, EntityProperty>();
            if (name.ToSensorType() == typeof(string))
            {
                dictionary.Add(name, new EntityProperty((string)value));
            }
            if (name.ToSensorType() == typeof(int))
            {
                if (value.GetType() == typeof(Int64))
                {
                    dictionary.Add(name, new EntityProperty((Int64)value));
                }
                else
                {
                    dictionary.Add(name, new EntityProperty((Int32)value));
                }
            }
            if (name.ToSensorType() == typeof(double))
            {
                dictionary.Add(name, new EntityProperty((double)value));
            }
            if (name.ToSensorType() == typeof(DateTime))
            {
                dictionary.Add(name, new EntityProperty((DateTime)value));
            }
            if (name.ToSensorType() == typeof(bool))
            {
                dictionary.Add(name, new EntityProperty((bool)value));
            }
            if (name.ToSensorType() == typeof(byte[]))
            {
                dictionary.Add(name, new EntityProperty((byte[])value));
            }
            return new DynamicTableEntity(time.GeneratePartitionKey(), time.GetTicks(), "*", dictionary);
        }
        public void IngestSensorValues()
        {
            IEnumerable<CloudQueueMessage> queue_messages = this.GetDigestMessages(32);
            if (queue_messages != null)
            {
                IEnumerable<IStorageDigestMessage> messages =
                    queue_messages.Select((q) => JsonConvert
                        .DeserializeObject<IStorageDigestMessage>(q.AsString, jss)).OrderBy(m => m.Time);
                IEnumerable<IGrouping<string, IStorageDigestMessage>> message_groups = messages
                    .GroupBy(m => m.Device.Id.ToUrn());
                Parallel.ForEach(message_groups, message_group =>
                {
                    Log.Partition();
                    foreach (IStorageDigestMessage m in message_group.OrderBy(mg => mg.Time))
                    {                        
                        this.IngestSensorValues(m);
                        
                    }
                });
            }
        }

        public void IngestSensorValues(IStorageDigestMessage message)
        {
            CloudTable device_channel = this.TableClient.GetTableReference(message.Device.Id.ToDeviceChannelTableName());            
            device_channel.CreateIfNotExists();
            Parallel.ForEach(message.SensorValues, sv =>
            {
                TableOperation insert_operation = TableOperation
                    .InsertOrMerge(AzureStorage.CreateSensorValueEntity(message.Time, sv.Key, sv.Value));
                TableResult result = device_channel.Execute(insert_operation);
                IStorageSensor sensor = message.Device.Sensors.Where(s => s.Key == sv.Key).FirstOrDefault().Value;
                Parallel.ForEach(sensor.Channels, c =>
                {
                    Log.Partition();
                    TableOperation channel_insert_operation = TableOperation
                        .InsertOrMerge(AzureStorage.CreateChannelItemEntity(message.Time, sv.Key, sv.Value));
                    CloudTable channel = this.TableClient.GetTableReference(c.ToChannelTableName());
                    channel.CreateIfNotExists();
                    TableResult channel_insert_result = device_channel.Execute(channel_insert_operation);
                });
            });            
        }
    }
}
