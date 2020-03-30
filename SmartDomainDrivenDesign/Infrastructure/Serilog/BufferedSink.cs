using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDomainDrivenDesign.Infrastructure.Serilog
{
    /// <summary>
    /// Sinks that buffers logs per requestId and full dumps it when a level is reached.
    /// </summary>
    public class BufferedSink : ILogEventSink, IDisposable
    {
        private readonly ILogEventSink wrappedSink;
        private readonly LogEventLevel minimumLevel;
        private readonly LogEventLevel fullDumpLevel;
        private readonly LoggingLevelSwitch controlLevelSwitch;
        private readonly ConcurrentDictionary<string, BlockingCollection<LogEvent>> buffers;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wrappedSink"></param>
        /// <param name="minimumLevel"></param>
        /// <param name="fullDumpLevel"></param>
        /// <param name="controlLevelSwitch"></param>
        public BufferedSink(ILogEventSink wrappedSink, LogEventLevel minimumLevel, LogEventLevel fullDumpLevel, LoggingLevelSwitch controlLevelSwitch)
        {
            this.wrappedSink = wrappedSink;
            this.minimumLevel = minimumLevel;
            this.fullDumpLevel = fullDumpLevel;
            this.controlLevelSwitch = controlLevelSwitch;

            this.buffers = new ConcurrentDictionary<string, BlockingCollection<LogEvent>>(20, 50);
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            /* If there's no RequestId, don't buffer the logs. Maybe I can buffer on different/many properties */
            if (!logEvent.Properties.TryGetValue("RequestId", out LogEventPropertyValue requestIdProperty))
            {
                wrappedSink.Emit(logEvent);
                return;
            }

            string requestId = requestIdProperty.ToString();

            /* If there's no buffer already, create it. GetOrAdd for concurrent logging and avoid race conditions. */
            if (!buffers.TryGetValue(requestId, out BlockingCollection<LogEvent> requestLogBuffer))
            {
                requestLogBuffer = buffers.GetOrAdd(requestId, (key) => new BlockingCollection<LogEvent>());
            }

            try
            {
                if (!requestLogBuffer.TryAdd(logEvent))
                {
                    SelfLog.WriteLine("{0} unable to enqueue, capacity {1}", typeof(BufferedSink), requestLogBuffer.BoundedCapacity);
                }
            }
            catch (InvalidOperationException)
            {
                SelfLog.WriteLine("Already completed queue: {0}", requestId);
            }

            /* Check if its the last log in a request by checking 
             * the SourceContext is Serilog.AspNetCore.RequestLoggingMiddleware */
            if (logEvent.Properties.TryGetValue("SourceContext", out LogEventPropertyValue sourceContext)
                && "Serilog.AspNetCore.RequestLoggingMiddleware".Equals((sourceContext as ScalarValue)?.Value))
            {
                FinishBuffer(requestLogBuffer);
                buffers.Remove(requestId, out _);
            }
            else if (logEvent.Level >= fullDumpLevel)
            {
                /* Remove until we find the event that triggered the Full Dump. 
                 * This way we ignore new logs added after logEvent was emitted. */
                while (requestLogBuffer.TryTake(out LogEvent log))
                {
                    wrappedSink.Emit(log);

                    if (object.ReferenceEquals(log, logEvent)) break;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var requestLogBuffer in buffers.Values)
            {
                FinishBuffer(requestLogBuffer);
            }

            buffers.Clear();
            (wrappedSink as IDisposable)?.Dispose();
        }

        /// <summary>
        /// Finishes and Emits all logs in a buffer.
        /// </summary>
        /// <param name="requestLogs"></param>
        private void FinishBuffer(BlockingCollection<LogEvent> requestLogs)
        {
            requestLogs.CompleteAdding();

            foreach (LogEvent log in requestLogs)
            {
                if (IsEnabled(log.Level))
                    wrappedSink.Emit(log);
            }

            requestLogs.Dispose();
        }

        /// <summary>
        /// Determine if events at the specified level, and higher, will be passed through
        /// to the log sinks.
        /// </summary>
        /// <param name="level">Level to check.</param>
        /// <returns>True if the level is enabled; otherwise, false.</returns>
        public bool IsEnabled(LogEventLevel level) => level >= (controlLevelSwitch?.MinimumLevel ?? minimumLevel);
    }
}
