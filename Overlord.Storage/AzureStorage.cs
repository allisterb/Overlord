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

using AutoMapper;
using Newtonsoft.Json;

using Overlord.Security;

using Overlord.Security.ClaimTypes;
using Overlord.Security.Claims;
using Overlord.Storage.Common;



namespace Overlord.Storage
{
    public partial class AzureStorage: IStorage
    {        
        #region Private fields        
        private AzureStorageEventSource Log = AzureStorageEventSource.Log;        
        private CloudStorageAccount _StorageAccount;        
        private CloudTableClient _TableClient;
        private CloudQueueClient _QueueClient;
        private CloudTable _UsersTable;
        private CloudTable _DevicesTable;
        private CloudTable _DeviceChannelTable;
        private CloudTable _SensorReadingsTable;
        private CloudTable _ChannelsTable;
        private CloudTable _AlertsTable;
        private CloudTable _MessagesTable;
        private CloudQueue _DigestQueue;

        JsonSerializerSettings jss = new JsonSerializerSettings();
        #endregion

        #region Constructors
        public AzureStorage()
        {
            this.jss.Converters.Add(new GuidConverter());
            this.UserEntityResolverFunc = (string partitionKey, string rowKey, DateTimeOffset timestamp, 
                IDictionary<string, EntityProperty> properties, string etag) =>
                {
                    return UserEntityResolver(partitionKey, rowKey, timestamp, properties, etag);
                };
            this.DeviceEntityResolverFunc = (string partitionKey, string rowKey,
                DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag) =>
                {
                    return DeviceEntityResolver(partitionKey, rowKey, timestamp, properties, etag);
                };
            this.ChannelEntityResolverFunc = (string partitionKey, string rowKey,
                DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag) =>
            {
                return ChannelEntityResolver(partitionKey, rowKey, timestamp, properties, etag);
            };             
        }       
        #endregion

        #region Destructors
        ~AzureStorage()
        {
         
        }
        
        #endregion

        #region Private properties
        private CloudStorageAccount StorageAccount
        {
            get
            {
                if (this._StorageAccount == null)
                {
                    try
                    {
                        #if DEBUG
                        this._StorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                        #else
                        this._StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager
                            .GetSetting("StorageConnectionString"));
                        #endif                        
                    }
                    catch (StorageException e)
                    {
                        Log.ConfigurationFailure("Couldn't read storage account from file.", e);                        
                        throw;
                    }
                    AzureStorageEventSource.Log.ConfigurationSuccess("Read storage account from file.");
                }
                return this._StorageAccount;
            }
            set
            {
                this._StorageAccount = value;
            }
        }

        private CloudTableClient TableClient
        {
            get
            {
                if (this._TableClient == null)
                {
                    try
                    {
                        this._TableClient = this.StorageAccount.CreateCloudTableClient();
                        Log.ConnectSuccess("Created Table Client.");
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Couldn't create Table Client.", e);
                        throw;
                    }                    
                    this._TableClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan
                        .FromSeconds(5), 7);                    
                }
                return this._TableClient;
            }
            set
            {
                this._TableClient = value;
            }
        }

        private CloudQueueClient QueueClient
        {
            get
            {
                if (this._QueueClient == null)
                {
                    try
                    {
                        this._QueueClient = this.StorageAccount.CreateCloudQueueClient();
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Failed to create queue storage client.", e);
                        throw;
                    }
                }
                return this._QueueClient;
            }
            set
            {
                this._QueueClient = value;
            }
        }

        private CloudTable UsersTable
        {
            get
            {
                if (this._UsersTable == null)
                {
                    try
                    {
                        this._UsersTable = this.TableClient.GetTableReference("Users");
                        this._UsersTable.CreateIfNotExists();
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Failed to connect to Users table.", e);
                        throw;
                    }                    
                }                
                return this._UsersTable;
            }
            set
            {
                this._UsersTable = value;
            }
        }

        private CloudTable DevicesTable
        {
            get
            {
                if (this._DevicesTable == null)
                {
                    try
                    {
                        this._DevicesTable = this.TableClient.GetTableReference("Devices");
                        this._DevicesTable.CreateIfNotExists();
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Failed to connect to Devices table.", e);
                        throw;
                    }
                }
                return this._DevicesTable;
            }
            set
            {
                this._DevicesTable = value;
            }
        }

        private CloudTable DeviceChannelTable
        {
            get
            {
                if (this._DeviceChannelTable == null)
                {
                    try
                    {
                        this._DeviceChannelTable = this.TableClient
                            .GetTableReference("Channel_" + 
                                OverlordIdentity.CurrentDeviceId.DeviceIdToTableName());
                        this._DeviceChannelTable.CreateIfNotExists();
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Failed to connect to " + 
                            "Channel_" + OverlordIdentity.CurrentDeviceId.DeviceIdToTableName() +
                        "device channel table.", e);
                        throw;
                    }
                }
                return this._DeviceChannelTable;
            }
            set
            {
                this._DeviceChannelTable = value;
            }
        }

        private CloudTable SensorReadingsTable
        {
            get
            {
                if (this._SensorReadingsTable == null)
                {
                    try
                    {
                        this._SensorReadingsTable = this.TableClient.GetTableReference("Readings");
                        this._SensorReadingsTable.CreateIfNotExists();
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Failed to connect to Readings table.", e);
                        throw;
                    }
                }
                return this._SensorReadingsTable;
            }
            set
            {
                this._SensorReadingsTable = value;
            }
        }

        private CloudTable ChannelsTable
        {
            get
            {
                if (this._ChannelsTable == null)
                {
                    try
                    {
                        this._ChannelsTable = this.TableClient.GetTableReference("Channels");
                        this._ChannelsTable.CreateIfNotExists();
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Failed to connect to Channels table.", e);
                        throw;
                    }
                }
                return this._ChannelsTable;
            }
            set
            {
                this._ChannelsTable = value;
            }
        }

        private CloudTable AlertsTable
        {
            get
            {
                if (this._AlertsTable == null)
                {
                    try
                    {
                        this._AlertsTable = this.TableClient.GetTableReference("Alerts");
                        this._AlertsTable.CreateIfNotExists();
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Failed to connect to Alerts table.", e);
                        throw;
                    }
                }
                return this._AlertsTable;
            }
            set
            {
                this._AlertsTable = value;
            }
        }

        private CloudQueue DigestQueue
        {
            get
            {
                if (this._DigestQueue == null)
                {
                    try
                    {
                        this._DigestQueue = this.QueueClient.GetQueueReference("digest");
                        this._DigestQueue.CreateIfNotExists();
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Failed to connect to Digest queue.", e);
                        throw;
                    }
                }
                return this._DigestQueue;
            }
            set
            {
                this._DigestQueue = value;
            }
        }        
        #endregion
        
        #region Public methods               
        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Anonymous)]        
        public bool AuthenticateAnonymousUser(string user_id, string user_token)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(user_id, 
                user_token);
            try
            {
                DynamicTableEntity user_entity = (DynamicTableEntity)this.UsersTable.Execute
                    (retrieveOperation).Result;
                if (user_entity == null)
                {
                    return false;
                }
                else
                {
                    IStorageUser user = this.UserEntityResolver(user_entity.PartitionKey, 
                        user_entity.RowKey, user_entity.Timestamp, user_entity.Properties, user_entity.ETag);
                    OverlordIdentity.InitializeUserIdentity(user.Id.ToUrn(), user.Token, 
                        user.Devices.Select(d => d.ToUrn()).ToList<String>());                        
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.ReadTableFailure(string.Format("Failed to read user entity: Id: {0}, Token: {1}.", 
                    user_id, user_token), e);
                throw;
            }
            finally
            {
                
            }

            
            /*
            TableOperation retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(urn_id, token);
            TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, urn_id.UrnToId()),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, token)
                )
            );
            IEnumerable<IStorageUser> user = this.UsersTable.ExecuteQuery<DynamicTableEntity, IStorageUser>(query, this.UserEntityResolverFunc);
            return user.FirstOrDefault(); 
             */
            //return null;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Anonymous)]
        public bool AuthenticateAnonymousDevice(string device_id, string device_token)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(device_id, 
                device_token);
            try
            {
                DynamicTableEntity device_entity = (DynamicTableEntity)this.DevicesTable.Execute
                    (retrieveOperation).Result;
                if (device_entity == null)
                {
                    return false;
                }
                else
                {
                    IStorageDevice device = this.DeviceEntityResolver(device_entity.PartitionKey, 
                        device_entity.RowKey, device_entity.Timestamp, device_entity.Properties,
                        device_entity.ETag);
                    OverlordIdentity.InitializeDeviceIdentity(device_id, device_token, 
                        device.Sensors.Select(s => s.Key).ToArray<string>());
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.ReadTableFailure(string.Format
                    ("Failed to retrieve device entity: Id: {0}, Token: {1}.", device_id, device_token), e);
                throw;
            }            
        }


     
        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Administrator)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.AddUser)]
        public IStorageUser AddUser (string name, string token, GeoIp geo_ip, string id = null)
        {
                                                        
            IStorageUser user = new IStorageUser()
            {
                Id = string.IsNullOrEmpty(id) ? Guid.NewGuid() : id.UrnToGuid(),
                Token = token,
                UserName = name,
                Devices = new List<Guid>()
            };
            TableOperation insertOperation = TableOperation.Insert(AzureStorage.CreateUserTableEntity(user));
            TableResult result;
            try
            {
                result = this.UsersTable.Execute(insertOperation);
                    Log.WriteTableSuccess(string.Format("Added user entity: {0}, Id: {1}, Token {2}.", 
                        user.UserName, user.Id.ToUrn(), user.Token));
                    return user;
                
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to add user entity: {0}, Id: {1}, Token {2}.", 
                    user.UserName, user.Id.ToUrn(), user.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.AddUser);
            }

        }
        
        
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.FindUser)]
        public IStorageUser FindUser(Guid id, string token)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(id.ToUrn(), token);
            try
            {
                DynamicTableEntity user_entity = (DynamicTableEntity)this.UsersTable
                    .Execute(retrieveOperation).Result;
                if (user_entity == null)
                {
                    return null;
                }
                else
                {
                    IStorageUser user = this.UserEntityResolver(user_entity.PartitionKey, user_entity.RowKey, user_entity.Timestamp, 
                        user_entity.Properties, user_entity.ETag);

                    return user;
                }
            }
            catch (Exception e)
            {
                Log.ReadTableFailure(string.Format("Failed to find user entity: Id: {0}, Token: {1}.", 
                    id.ToUrn(), token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.FindUser);
            }

        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage,
            Operation = StorageAction.FindUser)]
        public IStorageUser GetCurrentUser()
        {
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindUser);
            return this.FindUser(OverlordIdentity.CurrentUserId.UrnToGuid(), OverlordIdentity.CurrentUserToken);
        }


        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.UpdateUser)]
        public IStorageUser UpdateUser(IStorageUser user)
        {
            TableOperation update_user_operation = TableOperation.Merge(CreateUserTableEntity(user));
            try
            {
                TableResult result = this.UsersTable.Execute(update_user_operation);
                Log.WriteTableSuccess(string.Format("Updated user entity: {0}, Id: {1}, Token: {2}",
                    user.UserName, user.Id.ToUrn(), user.Token, user.Id.ToUrn()));
                return user;
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to updated user entity: Id: {0}, Token: {1}.",
                    user.Id.ToUrn(), user.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.UpdateUser);
            }                                
        }

        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Administrator)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.DeleteUser)]
        public bool DeleteUser(IStorageUser user)
        {
            TableOperation delete_user_operation = TableOperation.Delete(CreateUserTableEntity(user));
            try
            {
                TableResult result = this.UsersTable.Execute(delete_user_operation);
                Log.WriteTableSuccess(string.Format("Deleted user entity: {0}, Id: {1}, Token: {2}",
                    user.UserName, user.Id.ToUrn(), user.Token, user.Id.ToUrn()));
                return true;
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to delete user entity: Id: {0}, Token: {1}.",
                    user.Id.ToUrn(), user.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.DeleteUser);
            }                                

        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.AddDevice)]
        public IStorageDevice AddDevice(IStorageUser user, string name, string token, GeoIp location, 
            string id = null)
        {             
            IStorageDevice device = new IStorageDevice()
            {
                Id = string.IsNullOrEmpty(id) ? Guid.NewGuid() : id.UrnToGuid(),
                UserId = user.Id,
                Token = token,
                Name = name,
                Sensors = new Dictionary<string, IStorageSensor>()
            };
            
            try
            {                
                TableOperation insert_device_operation = TableOperation
                    .Insert(AzureStorage.CreateDeviceTableEntity(device));                                
                TableResult result;
                result = this.DevicesTable.Execute(insert_device_operation);
                device.ETag = result.Etag;
                user.Devices.Add(device.Id);
                TableOperation update_user_operation = TableOperation.Merge(CreateUserTableEntity(user));
                result = this.UsersTable.Execute(update_user_operation);
                user.ETag = result.Etag;
                Log.WriteTableSuccess(string.
                    Format("Added device entity: {0}, Id: {1}, Token {2} to Devices table.", 
                        device.Name, device.Id.ToUrn(), device.Token));                
                Log.WriteTableSuccess(string.Format("Added device entity: {0}, Id: {1}, to User entity {2}.", 
                    device.Name, device.Id.ToUrn(), device.Token, user.Id.ToUrn()));
                return device;

            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to add device entity: {0}, Id: {1}, Token {2}.",
                    device.Name, device.Id.ToUrn(), device.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.AddDevice);
            }
        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage,
            Operation = StorageAction.AddDevice)]
        public IStorageDevice AddDevice(string name, string token, GeoIp location,
            string id = null)
        {
            IStorageUser user = this.GetCurrentUser();
            return this.AddDevice(user, name, token, location);
        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.FindDevice)]
        public IStorageDevice FindDevice(Guid id, string token)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(id.ToUrn(), 
                token);
            try
            {
                DynamicTableEntity device_entity = (DynamicTableEntity)this.DevicesTable
                    .Execute(retrieveOperation).Result;
                if (device_entity == null)
                {
                    return null;
                }               
                return this.DeviceEntityResolver(device_entity.PartitionKey, device_entity.RowKey, 
                    device_entity.Timestamp, device_entity.Properties, device_entity.ETag);
            }
            catch (Exception e)
            {
                Log.ReadTableFailure(string.Format("Failed to read table for device: Id: {0}, Token: {1}.", 
                    id.ToUrn(), token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.FindDevice);
            }

        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.FindDevice)]
        public IStorageDevice GetCurrentDevice()
        {
            IStorageDevice device = this.FindDevice(OverlordIdentity.CurrentDeviceId.ToGuid(), 
                OverlordIdentity.CurrentDeviceToken);
            if (device == null) throw new NullReferenceException("Could not find current device id.");
            return device;
        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, Operation = StorageAction.UpdateDevice)]
        public IStorageDevice UpdateDevice(IStorageDevice device)
        {
            TableOperation update_device_operation = TableOperation.Merge(CreateDeviceTableEntity(device));
            try
            {
                TableResult result = this.DevicesTable.Execute(update_device_operation);
                Log.WriteTableSuccess(string.Format("Updated device entity: {0}, Id: {1}, Token: {2}",
                    device.Name, device.Id.ToUrn(), device.Token, device.Id.ToUrn()));
                device.ETag = result.Etag;
                return device;
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to update device entity: Id: {0}, Token: {1}.",
                    device.Id.ToUrn(), device.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.UpdateDevice);
            }                                
        }
        
        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Device)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, 
            Operation = StorageAction.AddSensor)]
        public IStorageSensor AddSensor(string sensor_name, string sensor_units, 
            IList<Guid> sensor_channels, IList<IStorageAlert> sensor_alerts)
        {
            if (!sensor_name.IsVaildSensorName()) throw new ArgumentException(
                string.Format("Invalid sensor name: {0}", sensor_name));
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindDevice);
            IStorageDevice device = this.GetCurrentDevice();            
            IStorageSensor sensor = new IStorageSensor()
            {
                DeviceId  = device.Id,
                Name = sensor_name,
                Unit = sensor_units,
                Channels = sensor_channels,
                Alerts = sensor_alerts
            };
            
            if (device.Sensors.Keys.Contains(sensor_name))
            {
                device.Sensors.Remove(sensor_name);
            }            
            device.Sensors.Add(sensor_name, sensor);            
            try
            {
                OverlordIdentity.AddClaim(Resource.Storage, StorageAction.UpdateDevice);
                this.UpdateDevice(device);
                Log.WriteTableSuccess(string.Format("Added sensor {0} to device entity: Id: {1}, Token: {2}",
                    sensor.Name, device.Id.ToUrn(), device.Token));
                return sensor;
            }
            catch (Exception e)
            {
                Log.ReadTableFailure(string.Format("Failed to read table for device: Id: {0}, Token: {1}.", 
                    device.Id.ToUrn(), device.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.AddSensor);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Device)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage,
            Operation = StorageAction.AddChannel)]
        public IStorageChannel AddChannel(string channel_name, string channel_description, 
            string sensor_type, string channel_units, List<IStorageAlert> alerts)        
        {
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.FindDevice);
            IStorageDevice device = this.GetCurrentDevice();

            IStorageChannel channel = new IStorageChannel()
            {
                Id = Guid.NewGuid(),                                
                Name = channel_name,
                Description = channel_description,  
                SensorType = sensor_type,
                Alerts = alerts              
            };
            try
            {
                TableOperation insert_channel_operation = TableOperation
                    .Insert(AzureStorage.CreateChannelTableEntity(channel));
                TableResult result;                                
                result = this.ChannelsTable.Execute(insert_channel_operation);                
                Log.WriteTableSuccess(string.Format("Added Channel entity: {0}, Id: {1}.",
                    channel.Name, channel.Id.ToUrn()));
                return channel;
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to add Channel entity: {0}, Id: {1}.", channel.Name, 
                    channel.Id), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.AddChannel);
            }
        }           
        #endregion

        #region Internal methods
        internal IStorageUser UserEntityResolver(string partitionKey, string rowKey,
        DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag)
        {
            IStorageUser user = new IStorageUser();
            user.Id = Guid.ParseExact(partitionKey, "D");
            user.Token = rowKey;
            user.ETag = etag;
            user.UserName = properties["UserName"].StringValue;
            user.Devices = JsonConvert.DeserializeObject<IList<Guid>>
                (properties["Devices"].StringValue);
            return user;
        }
        internal EntityResolver<IStorageUser> UserEntityResolverFunc;

        internal IStorageDevice DeviceEntityResolver(string partitionKey, string rowKey,
        DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag)
        {
            IStorageDevice device = new IStorageDevice();
            device.Id = Guid.ParseExact(partitionKey, "D");
            device.Token = rowKey;
            device.ETag = etag;
            device.UserId = properties["UserId"].GuidValue.Value;
            device.Name = properties["Name"].StringValue;
            device.Description = properties.Keys.Contains("Description") ? 
                properties["Description"].StringValue : null;
             device.Sensors = JsonConvert.DeserializeObject<IDictionary<string, IStorageSensor>>
                (properties["Sensors"].StringValue);
            device.Location =  properties.Keys.Contains("Location") ? 
                JsonConvert.DeserializeObject<Common.GeoIp>(properties["Location"].StringValue) : null;            
            return device;
        }
        internal EntityResolver<IStorageDevice> DeviceEntityResolverFunc;

        internal IStorageChannel ChannelEntityResolver(string partitionKey, string rowKey,
        DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag)
        {
            IStorageChannel channel = new IStorageChannel();
            channel.Id = Guid.ParseExact(partitionKey, "D");
            channel.ETag = etag;
            channel.Name = properties["Name"].StringValue;
            channel.SensorType = properties["SensorType"].StringValue;
            channel.Description = properties.Keys.Contains("Description") ?
                properties["Description"].StringValue : null;
            channel.Alerts = properties.Keys.Contains("Alerts") ?
                JsonConvert.DeserializeObject<IList<IStorageAlert>>(properties["Alerts"].StringValue) : null;
            return channel;
        }
        internal EntityResolver<IStorageChannel> ChannelEntityResolverFunc;
        #endregion
    
        #region Internal static methods
        internal static DynamicTableEntity CreateUserTableEntity(IStorageUser user)
        {
            Dictionary<string, EntityProperty> dictionary = new Dictionary<string, EntityProperty>();
            dictionary.Add("UserName", new EntityProperty(user.UserName));
            dictionary.Add("Devices", new EntityProperty(JsonConvert.SerializeObject(user.Devices)));
          
            return new DynamicTableEntity(user.Id.ToUrn(), user.Token, user.ETag, dictionary);
        }
        
        internal static DynamicTableEntity CreateDeviceTableEntity(IStorageDevice device)
        {
            Dictionary<string, EntityProperty> dictionary = new Dictionary<string, EntityProperty>();
            dictionary.Add("UserId", new EntityProperty(device.UserId));
            dictionary.Add("Name", new EntityProperty(device.Name));
            dictionary.Add("Description", new EntityProperty(device.Description));
            dictionary.Add("Location", new EntityProperty(JsonConvert.SerializeObject(device.Location)));
            string sensors_json = JsonConvert.SerializeObject(device.Sensors);
            dictionary.Add("Sensors", new EntityProperty(sensors_json));            
            return new DynamicTableEntity(device.Id.ToUrn(), device.Token, device.ETag, dictionary);
        }

        internal static DynamicTableEntity CreateChannelTableEntity(IStorageChannel channel)
        {
            var dictionary = new Dictionary<string, EntityProperty>();
            dictionary.Add("SensorType", new EntityProperty(channel.SensorType));
            dictionary.Add("Name", new EntityProperty(channel.Name));
            dictionary.Add("Description", new EntityProperty(channel.Description));
            dictionary.Add("Alerts", new EntityProperty(JsonConvert.SerializeObject(channel.Alerts)));                
            return new DynamicTableEntity(channel.Id.ToUrn(), channel.Id.ToUrn(), channel.ETag,
                dictionary);
        }

        #endregion
    }


}
