namespace BPMaster.Utilities
{
    public class DateTimeConvertUtil
    {
        public static DateTime GetCurrentTimeInUtc7()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var utc7Time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            return utc7Time;
        }

        public static DateTime GetCurrentTimeInUtc7ForMac()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
            var utc7Time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            return utc7Time;
        }

        public static DateTime ConvertTimeInUtc7(DateTime time)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var utc7Time = TimeZoneInfo.ConvertTimeFromUtc(time, timeZone);
                return utc7Time;
            }
            catch
            {
                return time;
            }          
        }

        public static DateTime ConvertTimeInUtc7ForMac(DateTime time)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
                var utc7Time = TimeZoneInfo.ConvertTimeFromUtc(time, timeZone);
                return utc7Time;
            }
            catch
            {
                return time;
            }
        }
    }
}
