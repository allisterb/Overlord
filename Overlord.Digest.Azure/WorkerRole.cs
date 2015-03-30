using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;

using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

using Overlord.Security;
using Overlord.Security.ClaimTypes;
using Overlord.Security.Claims;

using Overlord.Storage;
using Overlord.Digest;

namespace Overlord.Digest.Azure
{
    public class WorkerRole : RoleEntryPoint
    {
        #region Private fields
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private ObservableEventListener event_log_listener = new ObservableEventListener();
        private AzureDigestEventSource Log = AzureDigestEventSource.Log;
        #endregion

        #region Private properties

        #endregion
       
        public override void Run()
        {            
            Trace.TraceInformation("Overlord.Digest.Azure is running");
            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            event_log_listener.EnableEvents(Log, EventLevel.LogAlways,
              AzureDigestEventSource.Keywords.Perf | AzureDigestEventSource.Keywords.Diagnostic);
            EventTextFormatter formatter = new EventTextFormatter() { VerbosityThreshold = EventLevel.Error };
            event_log_listener.LogToFlatFile("Overlord.Digest.Azure.log", formatter, true);


      
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("Overlord.Digest.Azure has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Overlord.Digest.Azure is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Overlord.Digest.Azure has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                AzureStorage storage = new AzureStorage();
                IEnumerable<CloudQueueMessage> messages = await storage.GetDigestMessages(cancellationToken);

                //CloudQueueMessage message = await storage..GetMessageAsync(token);
                //if (message != null)
                //{
                //    IDigestQueueMessage message = JsonConvert.DeserializeObject<IDigestQueueMessage>(message);
                //    await _repository.CreateAsync(fixit);
                //    await queue.DeleteMessageAsync(message);
             }
                await Task.Delay(1000);
           
        }
    }
    
}
