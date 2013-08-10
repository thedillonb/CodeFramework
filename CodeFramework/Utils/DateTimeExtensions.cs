namespace System
{
    public static class DateTimeExtensions
    {
        public static string ToDaysAgo(this DateTime d)
        {
            var dt = DateTime.Now.Subtract(d.ToLocalTime());
            if (dt.TotalDays > 1)
                return Convert.ToInt32(dt.TotalDays) + " days ago".t();
            if (dt.TotalHours > 1)
                return Convert.ToInt32(dt.TotalHours) + " hours ago".t();
            if (dt.TotalMinutes > 1)
                return Convert.ToInt32(dt.TotalMinutes) + " minutes ago".t();
            return "moments ago".t();
        }

        public static int TotalDaysAgo(this DateTime d)
        {
            return Convert.ToInt32(Math.Round(DateTime.Now.Subtract(d.ToLocalTime()).TotalDays));
        }
    }
}

