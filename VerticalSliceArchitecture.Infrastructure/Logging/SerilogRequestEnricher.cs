using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace VerticalSliceArchitecture.Infrastructure.Logging
{
    public class SerilogRequestEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            try
            {
                // Retrieve the current OpenTelemetry Activity
                var activity = Activity.Current;

                if (activity != null)
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId));
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SerilogRequestEnricherFail: " + ex.Message);
            }
        }
    }

}