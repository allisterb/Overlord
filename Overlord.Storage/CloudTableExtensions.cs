using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Table;

using Overlord.Security;

using Overlord.Security.ClaimTypes;
using Overlord.Security.Claims;
using Overlord.Storage.Common;

namespace Overlord.Storage
{
    /*
    public static class CloudTableExtensions
    {
        public static string FindDevice(this CloudTable table)
        {
            string device_id = OverlordIdentity.GetClaim(ClaimTypes.DeviceId).FirstOrDefault();

            //TableOperation retrieveOperation = TableOperation.Retrieve<DynamicTableEntity>(id.ToUrn(), 
            //    token);
            try
            {
                DynamicTableEntity device_entity = (DynamicTableEntity)this.DevicesTable.Execute(retrieveOperation).Result;
                if (device_entity == null)
                {
                    return null;
                }               
                return this.DeviceEntityResolver(device_entity.PartitionKey, device_entity.RowKey, device_entity.Timestamp,
                    device_entity.Properties, device_entity.ETag);
        }
    }
     * */
}
