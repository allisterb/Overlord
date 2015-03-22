using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using Newtonsoft.Json;

namespace Overlord.Storage
{
    class AzureStorageReadingEntity : TableEntity
    {
        #region Private fields
        private const string RowKeyFormat = "{0}_{1}";
        #endregion

        #region Constructors
        public AzureStorageReadingEntity () { }

        public AzureStorageReadingEntity(DateTime reading_time, string user_name, string device_id, string sensor_name, object sensor_reading)
        {
            this.PartitionKey = reading_time.ToUniversalTime().GeneratePartitionKey();            
            this.RowKey = string.Format(
                CultureInfo.InvariantCulture,
                RowKeyFormat,
                device_id,
                sensor_name
            );
            this.DeviceReadingTime = reading_time;
            this.SensorName = sensor_name;
            this.UserName = user_name;
        }
        
        #endregion

        #region Private methods
        /// <summary>
        /// Create a key for the entity.
        /// </summary>
        /// <param name="sortKeysAscending"><see langword="true" /> generates WAD-style keys, otherwise it uses an key generated from a reversed tick value that is sorted from newest to oldest.</param>
        /// <param name="salt">The salt for the key.</param>
        private void CreateKey(bool sortKeysAscending, int salt)
        {
            this.PartitionKey = sortKeysAscending ? this.DeviceReadingTime.GeneratePartitionKey() : this.DeviceReadingTime.GeneratePartitionKeyReversed();

            this.RowKey = string.Format(
                CultureInfo.InvariantCulture,
                RowKeyFormat,
                this.UserName,
                sortKeysAscending ? this.DeviceReadingTime.GetTicks() : this.DeviceReadingTime.GetTicksReversed(),
                salt
            );        
        }
        
        #endregion

        #region Public properties        
        public string UserName { get; set; }
        public int DeviceId { get; set; }
        public string SensorName;
        public object DeviceReading { get; set; }
        public DateTime DeviceReadingTime { get; set; }        
        #endregion

        #region Public methods        
         
        
        #endregion

    }
}
