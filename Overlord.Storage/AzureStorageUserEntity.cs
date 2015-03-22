using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using Newtonsoft.Json;

using Overlord.Storage.Common;

namespace Overlord.Storage
{
    internal class AzureStorageUserEntity : TableEntity
    {
    
        #region Private fields
        private Common.GeoIp _GeoIP;
        
        #endregion

        #region Constructors

        public AzureStorageUserEntity () { }

        public AzureStorageUserEntity(string name, string token)
        {
            this.PartitionKey = name;
            this.RowKey = token;            
        }
        #endregion

        #region Public properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }        
        public string SerializedGeoIp { get; set; }
        public byte[] SerializedDevices { get; set; }
        
        #endregion
    }
    
    internal static class AzureStorageUserEntityEx
    {

       
    }
}


