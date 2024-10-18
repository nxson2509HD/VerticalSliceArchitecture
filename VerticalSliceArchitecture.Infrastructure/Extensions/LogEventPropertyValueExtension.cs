using Serilog.Events;

namespace VerticalSliceArchitecture.Infrastructure.Extensions
{
    public static class LogEventPropertyValueExtension
    {
        public static object? ExtractValue(this LogEventPropertyValue propertyValue)
        {
            try
            {
                if(propertyValue == null)
                {
                    return null;
                }

                switch (propertyValue)
                {
                    case ScalarValue scalarValue:
                    {
                        return scalarValue.Value;
                    }
                    default:
                    {
                        return propertyValue.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExtractValueFail: " + ex.Message);
            }

            return null;
        }
    }
}