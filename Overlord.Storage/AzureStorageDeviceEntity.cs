using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using Newtonsoft.Json;

namespace Overlord.Storage
{
    class AzureStorageDeviceEntity : TableEntity
    {
        #region Private fields
        private const string RowKeyFormat = "{0}_{1}";
        #endregion

        #region Constructors
        public AzureStorageDeviceEntity () { }

        public AzureStorageDeviceEntity(DateTime time, string user_name, string device_id, string sensor_name, object sensor_reading)
        {
            this.PartitionKey = time.ToUniversalTime().GeneratePartitionKey();            
            this.RowKey = string.Format(
                CultureInfo.InvariantCulture,
                RowKeyFormat,
                device_id,
                sensor_name
            );
        }
        
        #endregion
    }
}
