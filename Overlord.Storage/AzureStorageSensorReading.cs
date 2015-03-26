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
        /*
        public static DynamicTableEntity CreateSensorReadingEntity(Guid device_id, string sensor_name,
            DateTime time, object reading)
        {
            Dictionary<string, EntityProperty> dictionary = new Dictionary<string, EntityProperty>();
            dictionary.Add("DeviceId", new EntityProperty(device_id.ToUrn()));
            if (sensor_name.ToSensorType() == typeof(byte[]))
            {                
            dictionary.Add("UserId", new EntityProperty(device.UserId));
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

        }*/
    /*    
    [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Administrator)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Storage,
            Operation = StorageAction.AddUser)]
        public IStorageSensorReading AddSensorReading(string sensor_name, DateTime time, object reading
        {

            
    IStorageDevice device = this.FindDevice();

            if (!device.Sensors.Keys.Contains(sensor_name))
            {
                throw new ArgumentNullException("Sensor does not exist in device entity.");
            }
            
            IStorageSensorReading sensor_reading = new IStorageSensorReading()

            {
                
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
     * */
    }
}
