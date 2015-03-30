using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

using System.IdentityModel.Services;
using System.Security;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

using Overlord.Security;
using Overlord.Security.ClaimTypes;
using Overlord.Security.Claims;

using Overlord.Storage;

using Overlord.Core.Models;

namespace Overlord.Core
{
    public static class Api
    {

        #region Private fields
        private static ApiEventSource Log = ApiEventSource.Log;        
        #endregion

        static Api()
        {
            OverlordIdentity.InitializeAnonymousIdentity();
            var formatter = new EventTextFormatter() { VerbosityThreshold = EventLevel.Error };
            ObservableEventListener api_event_log_listener = new ObservableEventListener();
            api_event_log_listener.EnableEvents(ApiEventSource.Log, EventLevel.LogAlways,
                ApiEventSource.Keywords.Perf | ApiEventSource.Keywords.Diagnostic);
            api_event_log_listener.LogToFlatFile("Overlord.Core.Api.log", formatter, true);
            ObservableEventListener storage_event_log_listener = new ObservableEventListener();
            storage_event_log_listener.EnableEvents(AzureStorageEventSource.Log, EventLevel.LogAlways,
                AzureStorageEventSource.Keywords.Perf | AzureStorageEventSource.Keywords.Diagnostic);
            storage_event_log_listener.LogToFlatFile("Overlord.Storage.Azure.log", formatter, true);                        
        }

        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Anonymous)]        
        public static bool AuthenticateDevice(string id, string token)
        {
            AzureStorage storage = new AzureStorage();            
            return storage.AuthenticateAnonymousDevice(id.UrnToId(), token);            
        }
                
        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Device)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Api,
            Operation = ApiAction.AddReading)]
        public static void AddDeviceReading(DateTime time, IDictionary<string, object> values)
        {
            AzureStorage storage = new AzureStorage();
            OverlordIdentity.AddClaim(Resource.Storage, StorageAction.AddDeviceReading);
            storage.AddDeviceReading(time, values);
            Log.AddDeviceReadingSuccess("Added device readings.");
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = UserRole.User)]
        //public static void AddDevice()
        //{
        //    Guid uuid = new Guid();
        //    AzureStorage storage = new AzureStorage();
        //    storage.AddDevice(time, uuid.ToString().ToString(), name, geoip);
        //}
        /*
        [PrincipalPermission(SecurityAction.Demand, Role = UserRole.Device)]
        [ClaimsPrincipalPermission(SecurityAction.Demand, Resource = Resource.Api,
            Operation = ApiAction.AddReading)]
        public static void AddDeviceReading(DateTime time, Dictionary<string, object> values)
        {
            AzureStorage storage = new AzureStorage();
            storage.AddDeviceReading(time, values);
        }
        */
        //public static void FindUser(string user_name, string user_token, GeoIP user_geo_ip)
        //{
        //}
    }
}
