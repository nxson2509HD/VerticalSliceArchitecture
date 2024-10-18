using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using Serilog.Filters;
using Serilog.Events;
using VerticalSliceArchitecture.Infrastructure.Logging;
using VerticalSliceArchitecture.Infrastructure.Configurations;


namespace VerticalSliceArchitecture.Infrastructure.Configuration
{
    public static class SerilogConfiguration
    {
        public static void ConfigureSerilog(this IServiceCollection services, KafkaLoggingConfig kafkaLoggingConfig, string otlpEndpoint)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                //.Filter.ByExcluding( e =>
                //    Matching.FromSource("System.Net.Http.HttpClient.OtlpMetricExporter.ClientHandler")(e)
                //    || Matching.FromSource("System.Net.Http.HttpClient.OtlpMetricExporter.LogicalHandler")(e)
                //    || Matching.FromSource("System.Net.Http.HttpClient.OtlpTraceExporter.ClientHandler")(e)
                //    || Matching.FromSource("System.Net.Http.HttpClient.OtlpTraceExporter.LogicalHandler")(e)
                //)
                .Enrich.FromLogContext()
                .Enrich.With<SerilogRequestEnricher>()
                .WriteTo.File("wwwroot/logs/error/error.txt", LogEventLevel.Error, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // Error logs
                .WriteTo.File("wwwroot/logs/info/info.txt", LogEventLevel.Information, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // Info logs
                .WriteTo.File("wwwroot/logs/trace/traces.txt", LogEventLevel.Debug, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // Trace logs
                .WriteTo.Sink(new KafkaSink(kafkaLoggingConfig))
                .WriteTo.OpenTelemetry(opt => {
                    opt.Endpoint = otlpEndpoint + "/logs";
                    opt.Protocol = OtlpProtocol.HttpProtobuf;
                    opt.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = DiagnosticsConfig.ServiceName
                    };
                    opt.IncludedData = IncludedData.TraceIdField
                        | IncludedData.SpanIdField
                        | IncludedData.MessageTemplateTextAttribute
                        | IncludedData.MessageTemplateMD5HashAttribute;
                })
                .CreateLogger();

            Log.Logger = logger;

            services.AddLogging(loggingBuilder => {
                // Clear console and any other logging factory
                //loggingBuilder.ClearProviders();

                loggingBuilder
                    .AddSerilog(logger);
            });
        }

        private static Func<LogEvent, bool> FilterSourceContext(string sourceContext)
        {
            //return FilterFromProperty("SourceContext", sourceContext);
            return FilterFromProperty("System.SourceContext", sourceContext);
        }

        private static Func<LogEvent, bool> FilterFromProperty(string propertyName, string propValue)
        {
            return logEvent =>
            {
                try
                {
                    var getResult = logEvent.Properties.TryGetValue(propertyName, out var value);

                    if (getResult && value is ScalarValue sv)
                    {
                        var scalarValue = sv.Value;

                        if(scalarValue != null)
                        {
                            var result = (string)scalarValue == propValue;
                            return result;
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignoring the exception
                }

                return false;
            };
        }
    }
}