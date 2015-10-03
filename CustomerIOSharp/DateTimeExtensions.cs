namespace CustomerIOSharp
{
    using System;

    internal static class DateTimeExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();

        public static int ToUnixTimestamp(this DateTime value)
        {
            return Convert.ToInt32((value - Epoch).TotalSeconds);
        }

        public static DateTime ToDate(this int value)
        {
            return Epoch.AddSeconds(value);
        }
    }
}
