namespace VerticalSliceArchitecture.Infrastructure.Helper
{
    public static class DateTimeHelper
    {
        public static string FormatDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }

        public static DateTime ParseDateTime(string dateTimeString, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return DateTime.ParseExact(dateTimeString, format, null);
        }

        public static string GetCurrentDateTimeString(string format)
        {
            return DateTime.Now.ToString(format);
        }

        public static string ToShortDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string ToTimeOnly(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        public static string ToFullDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        }
    }
}
