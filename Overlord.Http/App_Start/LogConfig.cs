using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Tracing;
using System.Web;

using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;

using Overlord.Core;

namespace Overlord.Http
{
    public class LogConfig
    {
        public static void Configure()
        {
            var formatter = new EventTextFormatter() { VerbosityThreshold = EventLevel.Error };
            ObservableEventListener http_event_log_listener = new ObservableEventListener();
            http_event_log_listener.EnableEvents(HttpEventSource.Log, EventLevel.LogAlways,
                HttpEventSource.Keywords.Perf | HttpEventSource.Keywords.Diagnostic);
            http_event_log_listener.LogToFlatFile("Overlord.Http.log", formatter, true);
        }
    }
}