using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using System.Diagnostics;
using VerticalSliceArchitecture.Infrastructure.Configurations;

namespace VerticalSliceArchitecture.Infrastructure.Configuration
{

    public static class OpenTelemetryConfiguration
    {
        public static string SourceName { get; set; } = "SaleOrderServiceSourceName";
        public static void ConfigureOpentelemetry(this IServiceCollection services, string otlpEndpoint)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower();
            if(environment == null)
            {
                environment = "undefined";
            }

            services.AddOpenTelemetry()
                    .ConfigureResource(resourceBuilder => 
                        resourceBuilder
                            .AddService(DiagnosticsConfig.ServiceName)
                            .AddAttributes(new List<KeyValuePair<string, object>> { 
                                new("attribute1", "value1"),
                                //new(OpenTelemetryAttributeName.Service.Environment, environment),
                                //new("service.environment", environment),
                                new("deployment.environment", environment),
                            }))
                    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                        .AddSource(SourceName)
                        .ConfigureResource(resource => resource
                        .AddService(DiagnosticsConfig.ServiceName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new UriBuilder( otlpEndpoint + "/traces").Uri;
                            otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                        })
                    )
                    .WithMetrics(meterProviderBuilder => meterProviderBuilder
                        .AddMeter(DiagnosticsConfig.Meter.Name)
                        .ConfigureResource(resource => resource
                            .AddService(DiagnosticsConfig.ServiceName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new UriBuilder(otlpEndpoint + "/metrics").Uri;
                            otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                        })
                    );

            // Define Gauges for CPU Utilization and Memory Usage
            DiagnosticsConfig.Meter.CreateObservableGauge<double>("system.cpu.utilization", () => {
                var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                var value = cpuCounter.NextValue();

                //Note: In most cases you need to call .NextValue() twice to be able to get the real value
                if (Math.Abs(value) <= 0.00)
                    value = cpuCounter.NextValue();
                return value;
            }, "1", "no description");
            
            DiagnosticsConfig.Meter.CreateObservableGauge<double>("system.memory.utilization", () => {
                var theMemCounter = new PerformanceCounter("Memory", "Available MBytes");
                var value = theMemCounter.NextValue();

                if (Math.Abs(value) <= 0.00)
                    value = theMemCounter.NextValue();

                return value;
            }, "1");
        }
    }
}
