using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace VerticalSliceArchitecture.Infrastructure.Configurations {
    public class DiagnosticsConfig {
        public static void Initialize(string serviceName) {
            ServiceName = serviceName;
        }
        public static string ServiceName = "VerticalSliceArchitecture";
        public static Meter Meter = new Meter(ServiceName);
        public ActivitySource ActivitySource  = new ActivitySource(ServiceName);
    }
}

