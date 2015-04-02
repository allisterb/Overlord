using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

namespace Overlord.Http
{
    [EventSource(Name = "Http")]
    public class HttpEventSource : EventSource 
    {
        public class Keywords
        {
          public const EventKeywords Configuration = (EventKeywords)1;
          public const EventKeywords Diagnostic = (EventKeywords)2;
          public const EventKeywords Perf = (EventKeywords)4;
          public const EventKeywords Async = (EventKeywords)8;
          public const EventKeywords Security = (EventKeywords)16;
          public const EventKeywords Api = (EventKeywords)32;          
        }
 
        public class Tasks
        {
          public const EventTask Configure = (EventTask)1;
          public const EventTask AwaitTask = (EventTask)2;
          public const EventTask AsyncTask = (EventTask)4;
          public const EventTask Authenticate = (EventTask)8;
        }

        private static HttpEventSource _log = new HttpEventSource();
        private HttpEventSource() { }
        public static HttpEventSource Log { get { return _log; } }
 
    
        [Event(1, Message = "FAILURE: Read ASP.NET configuration: {0}\nException : {1}.", 
            Task = Tasks.Configure, Level = EventLevel.Critical, Keywords = Keywords.Diagnostic)]
        internal void ConfigurationFailure(string message, string exception)
        {
            if (this.IsEnabled()) this.WriteEvent(1, message, exception);
        }        
        [Event(2, Message = "SUCCESS: Read ASP.NET configuration: {0}", Task = Tasks.Configure,
            Level = EventLevel.Informational, Keywords = Keywords.Diagnostic)]
        internal void ConfigurationSuccess(string message)
        {
            this.WriteEvent(2, message);
        }

        [Event(3, Message = "FAILURE: Failed to authenticate client: {0}\nException: {1}",
            Task = Tasks.Authenticate, Level = EventLevel.Error, Keywords = Keywords.Diagnostic |
            Keywords.Security)]
        internal void AuthenticateFailure(string message, string exception)
        {
            this.WriteEvent(3, message, exception);
        }
                   
        [Event(4, Message = "SUCCESS: Authenticated device id: {0}, token: {1}. Access {2}.", 
            Task = Tasks.Authenticate, Level = EventLevel.Informational, 
            Keywords = Keywords.Diagnostic | Keywords.Security)]
        internal void AuthenticateDevice(string device_id, string device_token, bool access_granted)
        {
            this.WriteEvent(4, device_id, device_token, access_granted ? "Granted" : "Denied");
        }

        [Event(5, Message = "SUCCESS: Authenticated user id: {0}, token: {1}. Accesa {2}.",
         Task = Tasks.Authenticate, Level = EventLevel.Informational,
         Keywords = Keywords.Diagnostic | Keywords.Security)]
        internal void AuthenticateUser(string user_id, string user_token, bool access_granted)
        {
            this.WriteEvent(4, user_id, user_token, access_granted ? "Granted" : "Denied");
        }

        [Event(6, Message = 
            "AWAIT: Current Task: {0} executing on managed thread id: {1} with principal identity: {2}.", 
            Task = Tasks.AwaitTask, Level = EventLevel.Informational, 
            Keywords = Keywords.Diagnostic | Keywords.Async)]
        internal void Await(string message)
        {
            this.WriteEvent(5, message, Thread.CurrentThread.ManagedThreadId.ToString(),
                Thread.CurrentPrincipal.Identity.Name);
        }

        [Event(7, Message =
            "AWAIT: New Task: {0} executing on managed thread id: {1} with principal identity: {2}.",
            Task = Tasks.AwaitTask, Level = EventLevel.Informational,
            Keywords = Keywords.Diagnostic | Keywords.Async)]
        internal void Async(string message)
        {
            this.WriteEvent(5, message, Thread.CurrentThread.ManagedThreadId.ToString(),
                Thread.CurrentPrincipal.Identity.Name);
        }
               
    }

    public static class HttpEventExtensions
    {
        public static void ConfigurationFailure(this HttpEventSource ev, string message, Exception e)
        {
            ev.ConfigurationFailure(message, e.ToString());
        }

        public static void AuthenticationFailure(this HttpEventSource ev, string message, Exception e)
        {
            ev.AuthenticateFailure(message, e.ToString());
        }               
    }


}
