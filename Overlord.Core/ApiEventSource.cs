using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

namespace Overlord.Core
{
    [EventSource(Name = "CoreAPI")]
    public class ApiEventSource : EventSource 
    {
        public class Keywords
        {
          public const EventKeywords Configuration = (EventKeywords)1;          
          public const EventKeywords Diagnostic = (EventKeywords)2;
          public const EventKeywords Perf = (EventKeywords)4;
          public const EventKeywords Server = (EventKeywords)8;
          public const EventKeywords Device = (EventKeywords)16;
        }
 
        public class Tasks
        {
          public const EventTask Configure = (EventTask)1;
          public const EventTask Connect = (EventTask)2;
          public const EventTask AddDevice = (EventTask)4;
          public const EventTask AddDeviceReading = (EventTask)8;
        }


        private static ApiEventSource _log = new ApiEventSource();
        private ApiEventSource() { }
        public static ApiEventSource Log { get { return _log; } }
 
    
        [Event(1, Message = "FAILURE: Read Api configuration: {0}\nException : {1}.", Task = Tasks.Configure, 
            Level = EventLevel.Critical, Keywords = Keywords.Diagnostic)]
        internal void ConfigurationFailure(string message, string exception)
        {
            if (this.IsEnabled()) this.WriteEvent(1, message, exception);
        }
        
        [Event(2, Message = "SUCCESS: Read Api configuration: {0}", Task = Tasks.Configure,
            Level = EventLevel.Informational, Keywords = Keywords.Diagnostic)]
        internal void ConfigurationSuccess(string message)
        {
            this.WriteEvent(2, message);
        }

         
        [Event(3, Message = "FAILURE: Connect to remote server: {0}\nException: {1}", Task = Tasks.Connect, Level = EventLevel.Error,
            Keywords = Keywords.Diagnostic | Keywords.Server)]
        internal void ConnectFailure(string message, string exception)
        {
            this.WriteEvent(3, message, exception);
        }
          
        [Event(4, Message = "SUCCESS: Connect to remote server: {0}", Task = Tasks.Connect, Level = EventLevel.Informational, 
            Keywords = Keywords.Diagnostic | Keywords.Server)]
        internal void ConnectSuccess(string message)
        {
            this.WriteEvent(4, message);
        }

        [Event(5, Message = "FAILURE: Add Device: {0}\nException: {1}", Task = Tasks.AddDevice, Level = EventLevel.Error,
            Keywords = Keywords.Diagnostic | Keywords.Device)]
        internal void AddDeviceFailure(string message, string exception)
        {
            this.WriteEvent(5, message, exception);
        }

        [Event(6, Message = "SUCCESS: Add Device: {0}", Task = Tasks.AddDevice, Level = EventLevel.Informational,
            Keywords = Keywords.Diagnostic | Keywords.Device)]
        internal void AddDeviceSuccess(string message)
        {
            this.WriteEvent(6, message);
        }

        [Event(7, Message = "FAILURE: Add Device Reading: {0}\nException: {1}", Task = Tasks.AddDeviceReading, Level = EventLevel.Error,
            Keywords = Keywords.Diagnostic | Keywords.Device)]
        internal void AddDeviceReadingFailure(string message, string exception)
        {
            this.WriteEvent(7, message, exception);
        }

        [Event(8, Message = "SUCCESS: Add Device Reading: {0}", Task = Tasks.AddDeviceReading, Level = EventLevel.Informational,
            Keywords = Keywords.Diagnostic | Keywords.Device)]
        internal void AddDeviceReadingSuccess(string message)
        {
            this.WriteEvent(8, message);
        }

    }

    public static class ApiEventExtensions
    {
        public static void ConfigurationFailure(this ApiEventSource ev, string message, Exception e)
        {
            ev.ConfigurationFailure(message, e.ToString());
        }

        public static void ConnectFailure(this ApiEventSource ev, string message, Exception e)
        {
            ev.ConnectFailure(message, e.ToString());
        }

        public static void AddDeviceFailure(this ApiEventSource ev, string message, Exception e)
        {
            ev.AddDeviceFailure(message, e.ToString());
        }

        public static void AddDeviceReadingFailure(this ApiEventSource ev, string message, Exception e)
        {
            ev.AddDeviceReadingFailure(message, e.ToString());
        }


    }


}
