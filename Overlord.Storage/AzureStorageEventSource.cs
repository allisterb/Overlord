using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

using Microsoft.WindowsAzure.Storage;

namespace Overlord.Storage
{
    [EventSource(Name = "Storage")]
    public class AzureStorageEventSource : EventSource 
    {
        public class Keywords
        {
          public const EventKeywords Configuration = (EventKeywords)1;
          public const EventKeywords Diagnostic = (EventKeywords)2;
          public const EventKeywords Perf = (EventKeywords)4;
          public const EventKeywords Table = (EventKeywords)8;          
        }
 
        public class Tasks
        {
          public const EventTask Configure = (EventTask)1;
          public const EventTask Connect = (EventTask)2;
          public const EventTask WriteTable = (EventTask)4;
          public const EventTask ReadTable = (EventTask)8;
          public const EventTask AddDeviceEntity = (EventTask)16;
          public const EventTask FindDeviceEntity = (EventTask)32;
          public const EventTask UpdateDeviceEntity = (EventTask)64;
          public const EventTask DeleteDeviceEntity = (EventTask)128;
        }

        private static AzureStorageEventSource _log = new AzureStorageEventSource();
        private AzureStorageEventSource() { }
        public static AzureStorageEventSource Log { get { return _log; } }
 
    
        [Event(1, Message = "FAILURE: Read Azure configuration: {0}\nException : {1}.", Task = Tasks.Configure, 
            Level = EventLevel.Critical, Keywords = Keywords.Diagnostic)]
        internal void ConfigurationFailure(string message, string exception)
        {
            if (this.IsEnabled()) this.WriteEvent(1, message, exception);
        }
        
        [Event(2, Message = "SUCCESS: Read Azure configuration: {0}", Task = Tasks.Configure,
            Level = EventLevel.Informational, Keywords = Keywords.Diagnostic)]
        internal void ConfigurationSuccess(string message)
        {
            this.WriteEvent(2, message);
        }

         
        [Event(3, Message = "FAILURE: Connect to Azure servers: {0}\nException: {1}", Task = Tasks.Connect, Level = EventLevel.Error,
            Keywords = Keywords.Diagnostic | Keywords.Table)]
        internal void ConnectFailure(string message, string exception)
        {
            this.WriteEvent(3, message, exception);
        }
          
        [Event(4, Message = "SUCCESS: Connect to Azure servers: {0}", Task = Tasks.Connect, Level = EventLevel.Informational, 
            Keywords = Keywords.Diagnostic | Keywords.Table)]
        internal void ConnectSuccess(string message)
        {
            this.WriteEvent(4, message);
        }

        [Event(5, Message = "FAILURE: Write Azure Table Storage: {0}\nException: {1}", Task = Tasks.WriteTable, Level = EventLevel.Error,
            Keywords = Keywords.Diagnostic | Keywords.Table)]
        internal void WriteTableFailure(string message, string exception)
        {
            this.WriteEvent(5, message, exception);
        }

        [Event(6, Message = "SUCCESS: Write Azure Table Storage: {0}", Task = Tasks.WriteTable, Level = EventLevel.Informational,
            Keywords = Keywords.Diagnostic | Keywords.Table)]
        internal void WriteTableSuccess(string message)
        {
            this.WriteEvent(6, message);
        }

        [Event(7, Message = "FAILURE: Read Azure Table Storage: {0}\nException: {1}", Task = Tasks.ReadTable, Level = EventLevel.Error,
            Keywords = Keywords.Diagnostic | Keywords.Table)]
        internal void ReadTableFailure(string message, string exception)
        {
            this.WriteEvent(7, message, exception);
        }

        [Event(8, Message = "SUCCESS: Read Azure Table Storage: {0}", Task = Tasks.ReadTable, Level = EventLevel.Informational,
            Keywords = Keywords.Diagnostic | Keywords.Table)]
        internal void ReadTableSuccess(string message)
        {
            this.WriteEvent(8, message);
        }

    }

    public static class AzureStorageEventExtensions
    {
        public static void ConfigurationFailure(this AzureStorageEventSource ev, string message, Exception e)
        {
            ev.ConfigurationFailure(message, e.ToString());
        }

        public static void ConnectFailure(this AzureStorageEventSource ev, string message, Exception e)
        {
            ev.ConnectFailure(message, e.ToString());
        }

        public static void WriteTableFailure(this AzureStorageEventSource ev, string message, Exception e)
        {
            ev.WriteTableFailure(message, e.ToString());
        }

        public static void ReadTableFailure(this AzureStorageEventSource ev, string message, Exception e)
        {
            ev.ReadTableFailure(message, e.ToString());
        }



    }


}
