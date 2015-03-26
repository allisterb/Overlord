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
        private const string SensorReadingKeyFormat = "{0}_{1}_{2:X5}";

        public static DynamicTableEntity CreateSensorReadingEntity(IStorageSensorReading reading)
        {
            Dictionary<string, EntityProperty> dictionary = new Dictionary<string, EntityProperty>();
            dictionary.Add("DeviceId", new EntityProperty(reading.DeviceId.ToUrn()));
            dictionary.Add("Time", new EntityProperty(reading.Time));
            dictionary.Add("SensorName", new EntityProperty(reading.SensorName));
            if (reading.SensorName.ToSensorType() is string) dictionary.Add(reading.SensorName, 
                new EntityProperty((string)reading.Reading));
            if (reading.SensorName.ToSensorType() is int) dictionary.Add(reading.SensorName, 
                new EntityProperty((int)reading.Reading));
            if (reading.SensorName.ToSensorType() is double) dictionary.Add(reading.SensorName, 
                new EntityProperty((double)reading.Reading));
            if (reading.SensorName.ToSensorType() is DateTime) dictionary.Add(reading.SensorName, 
                new EntityProperty((DateTime)reading.Reading));
            if (reading.SensorName.ToSensorType() is bool) dictionary.Add(reading.SensorName, 
                new EntityProperty((bool)reading.Reading));
            if (reading.SensorName.ToSensorType() is byte[]) dictionary.Add(reading.SensorName, 
                new EntityProperty((byte[])reading.Reading));
            return new DynamicTableEntity(reading.Time.GeneratePartitionKey(), 
                string.Format(
                CultureInfo.InvariantCulture,
                SensorReadingKeyFormat,
                reading.DeviceId.ToUrn() + ":",
                reading.SensorName + ":",
                reading.Time.GetTicks()),
                null, dictionary);            
          }           
        }
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
