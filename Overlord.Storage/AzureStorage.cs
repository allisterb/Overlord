using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Permissions;
using System.IdentityModel.Services;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure;
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
    public class AzureStorage: IStorage
    {        
        #region Private fields
        ObservableEventListener event_log_listener = new ObservableEventListener();
        private AzureStorageEventSource Log = AzureStorageEventSource.Log;

        private string user_name;

        private CloudStorageAccount _StorageAccount;
        private OperationContext storage_operation_context;                
        private CloudTableClient _TableClient;
        private CloudQueueClient _QueueClient;
        private CloudTable _UsersTable;
        private CloudTable _DevicesTable;
        
        #endregion

        #region Constructors
        public AzureStorage()
        {            
            event_log_listener.EnableEvents(Log, EventLevel.LogAlways,
              AzureStorageEventSource.Keywords.Perf | AzureStorageEventSource.Keywords.Diagnostic);
            var formatter = new EventTextFormatter() { VerbosityThreshold = EventLevel.Error };
            event_log_listener.LogToConsole(formatter);            
 
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
                        this._StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
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
                        Log.ConnectSuccess("Couldn't create Table Client.");
                    }
                    catch (StorageException e)
                    {
                        Log.ConnectFailure("Couldn't create Table Client.", e);
                        throw;
                    }                    
                    this._TableClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(5), 7);                    
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
                        Log.ConnectFailure("Failed to Devices table.", e);
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
        
        #endregion

        #region Private methods
        internal EntityResolver<IStorageUser> UserEntityResolver = (string partitionKey, string rowKey,
        DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag) =>
        {
            IStorageUser user = new IStorageUser();
            user.Id = Guid.ParseExact(partitionKey, "X16");
            user.Token = rowKey;
            user.Version = etag;
            user.Devices = (IDictionary<string, IStorageDevice>)
                from p in properties
                where p.Key.StartsWith("Device_")
                select new IStorageDevice()
                {
                    Id = Guid.ParseExact(p.Key.Substring(p.Key.IndexOf("Device_")), "X16")
                };
            return user;
        };

        internal EntityResolver<IStorageDevice> DeviceEntityResolver = (string partitionKey, string rowKey,
        DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag) =>
        {
            IStorageDevice device = new IStorageDevice();
            device.Id = Guid.ParseExact(partitionKey, "X16");
            device.Token = rowKey;
            device.Version = etag;
            if (properties.Keys.Contains("UserId"))
            {
                device.UserId = properties["UserId"].GuidValue.Value;
            }
//            user.Devices = (IDictionary<string, IStorageDevice>)
//                from p in properties
//                where p.Key.StartsWith("Device_")
//                select new IStorageDevice()
//                {
//                    Id = Guid.ParseExact(p.Key.Substring(p.Key.IndexOf("Device_")), "X16")
//                };
            return device;
        };

        #endregion

        #region Public methods        
        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Administrator)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, Operation = StorageAction.AddUser)]
        public IStorageUser AddUser (string name, string token, GeoIp geo_ip)
        {
            IStorageUser user = new IStorageUser()
            {
                Id = Guid.NewGuid(),
                Token = token,
                UserName = name
            };
            TableOperation insertOperation = TableOperation.Insert(AzureStorage.CreateUserTableEntity(user));
            TableResult result;
            try
            {
                result = this.UsersTable.Execute(insertOperation);
                    Log.WriteTableSuccess(string.Format("Added user: {0}, Id: {1}, Token {2}.", user.UserName, user.Id.ToUrn(), user.Token));
                    return user;
                
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to add user: {0}, Id: {1}, Token {2}.", user.UserName, user.Id.ToUrn(), user.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.AddUser);
            }

        }
        
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, Operation = StorageAction.FindUser)]
        public IStorageUser FindUser(Guid id, string token)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(id.ToUrn(), token);

            try
            {
                DynamicTableEntity user_entity = (DynamicTableEntity)this.UsersTable.Execute(retrieveOperation).Result;
                if (user_entity == null)
                {
                    return null;
                }
                IStorageUser user = new IStorageUser();
                user.Id = Guid.ParseExact(user_entity.PartitionKey, "D");
                user.Token = user_entity.RowKey;
                user.Version = user_entity.ETag;
                EntityProperty name_property;
                user_entity.Properties.TryGetValue("UserName", out name_property);
                if (name_property == null)
                {
                    throw new NullReferenceException("Could not get UserName property.");
                }
                else
                {
                    user.UserName = name_property.StringValue;
                }
                return user;
            }
            catch (Exception e)
            {
                Log.ReadTableFailure(string.Format("Failed to read table for user: Id: {0}, Token: {1}.", id.ToUrn(), token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.FindUser);
            }

        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, Operation = StorageAction.UpdateUser)]
        public IStorageUser UpdateUser(IStorageUser user)
        {
            TableOperation update_user_operation = TableOperation.Merge(CreateUserTableEntity(user));
            try
            {
                TableResult result = this.UsersTable.Execute(update_user_operation);
                Log.WriteTableSuccess(string.Format("Updated user: {0}, Id: {1}, Token: {2}",
                    user.UserName, user.Id.ToUrn(), user.Token, user.Id.ToUrn()));
                return user;
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to updated user: Id: {1}, Token: {2}.",
                    user.Id.ToUrn(), user.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.UpdateUser);
            }                                
        }

        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Administrator)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, Operation = StorageAction.DeleteUser)]
        public bool DeleteUser(IStorageUser user)
        {
            TableOperation delete_user_operation = TableOperation.Delete(CreateUserTableEntity(user));
            try
            {
                TableResult result = this.UsersTable.Execute(delete_user_operation);
                Log.WriteTableSuccess(string.Format("Deleted user: {0}, Id: {1}, Token: {2}",
                    user.UserName, user.Id.ToUrn(), user.Token, user.Id.ToUrn()));
                return true;
            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to delete user: Id: {1}, Token: {2}.",
                    user.Id.ToUrn(), user.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.DeleteUser);
            }                                

        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, Operation = StorageAction.AddDevice)]
        public IStorageDevice AddDevice(IStorageUser user, string name, string token, GeoIp geoip)
        {
            IStorageDevice device = new IStorageDevice()
            {
                Id = Guid.NewGuid(),
                Token = token,
                Name = name
            };            
            try
            {
                TableOperation insert_device_operation = TableOperation.Insert(AzureStorage.CreateDeviceTableEntity(device));                
                TableOperation update_user_operation = TableOperation.Merge(CreateUserTableEntity(user));
                TableResult result;
                result = this.DevicesTable.Execute(insert_device_operation);
                user.Devices.Add(device.Id.ToUrn(), device);                                
                result = this.UsersTable.Execute(update_user_operation);
                Log.WriteTableSuccess(string.Format("Added device: {0}, Id: {1}, Token {2} to Devices table.", device.Name, device.Id.ToUrn(), device.Token));
                if (user.Devices == null)
                {
                    user.Devices = new Dictionary<string, IStorageDevice>();
                }
                
                Log.WriteTableSuccess(string.Format("Added device: {0}, Id: {1}, Token {2} to user {3}.", 
                    device.Name, device.Id.ToUrn(), device.Token, user.Id.ToUrn()));
                return device;

            }
            catch (Exception e)
            {
                Log.WriteTableFailure(string.Format("Failed to add device: {0}, Id: {1}, Token {2}.", device.Name, device.Id.ToUrn(), device.Token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.AddDevice);
            }
        }

        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage, Operation = StorageAction.FindDevice)]
        public IStorageDevice FindDevice(Guid id, string token)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(id.ToUrn(), token);
            try
            {
                DynamicTableEntity device_entity = (DynamicTableEntity)this.DevicesTable.Execute(retrieveOperation).Result;
                if (device_entity == null)
                {
                    return null;
                }
                IStorageDevice device = new IStorageDevice();
                device.Id = device_entity.PartitionKey.ToGuid();
                device.Token = device_entity.RowKey;
                EntityProperty name_property;
                device_entity.Properties.TryGetValue("Name", out name_property);
                if (name_property == null)
                {
                    throw new NullReferenceException("Could not get Name property.");
                }
                else
                {
                    device.Name = name_property.StringValue;
                }
                return device;
            }
            catch (Exception e)
            {
                Log.ReadTableFailure(string.Format("Failed to read table for devoce: Id: {0}, Token: {1}.", id.ToUrn(), token), e);
                throw;
            }
            finally
            {
                OverlordIdentity.DeleteClaim(Resource.Storage, StorageAction.FindDevice);
            }

        }

        #endregion
    
        #region Public static methods
        internal static DynamicTableEntity CreateUserTableEntity(IStorageUser user)
        {
            var dictionary = new Dictionary<string, EntityProperty>();
            dictionary.Add("UserName", new EntityProperty(user.UserName));
            if (user.Devices != null && user.Devices.Count > 0)
            {
                foreach (var device in user.Devices)
                {
                    string json = JsonConvert.SerializeObject(device.Value);
                    dictionary.Add(string.Format(CultureInfo.InvariantCulture, "Device_{0}", device.Key).Replace("-", "_"), new EntityProperty(json));
                }
            }

            return new DynamicTableEntity(user.Id.ToUrn(), user.Token, user.Version, dictionary);
        }

        internal static DynamicTableEntity CreateDeviceTableEntity(IStorageDevice device)
        {
            var dictionary = new Dictionary<string, EntityProperty>();
            dictionary.Add("Name", new EntityProperty(device.Name));
            if (device.Sensors != null && device.Sensors.Count > 0)
            {
                foreach (var sensor in device.Sensors)
                {
                    var type = sensor.Value.GetType();
                    if (type == typeof(byte[]))
                    {
                        string json = JsonConvert.SerializeObject((byte[])sensor.Value);
                        dictionary.Add(string.Format(CultureInfo.InvariantCulture, "Sensor_{0}", sensor.Key), new EntityProperty(json));
                    }
                    if (type == typeof(bool))
                    {
                        string json = JsonConvert.SerializeObject((bool)sensor.Value);
                        dictionary.Add(string.Format(CultureInfo.InvariantCulture, "Sensor_{0}", sensor.Key), new EntityProperty(json));
                    }

                    if (type == typeof(DateTime))
                    {
                        string json = JsonConvert.SerializeObject((DateTime)sensor.Value);
                        dictionary.Add(string.Format(CultureInfo.InvariantCulture, "Sensor_{0}", sensor.Key), new EntityProperty(json));
                    }
                    if (type == typeof(long))
                    {
                        string json = JsonConvert.SerializeObject((long)sensor.Value);
                        dictionary.Add(string.Format(CultureInfo.InvariantCulture, "Sensor_{0}", sensor.Key), new EntityProperty(json));
                    }
                    if (type == typeof(double))
                    {
                        string json = JsonConvert.SerializeObject((double)sensor.Value);
                        dictionary.Add(string.Format(CultureInfo.InvariantCulture, "Sensor_{0}", sensor.Key), new EntityProperty(json));
                    } 

                    if (type == typeof(string))
                    {
                        string json = JsonConvert.SerializeObject((string) sensor.Value);
                        dictionary.Add(string.Format(CultureInfo.InvariantCulture, "Sensor_{0}", sensor.Key), new EntityProperty(json));
                    } 
                    else if (type == typeof(int))
                    {
                        string json = JsonConvert.SerializeObject((int)sensor.Value);
                        dictionary.Add(string.Format(CultureInfo.InvariantCulture, "Sensor_{0}", sensor.Key), new EntityProperty(json));
                    }
                }
                
            }

            return new DynamicTableEntity(device.Id.ToUrn(), device.Token, device.Version, dictionary);
        }



        #endregion
    }


}
