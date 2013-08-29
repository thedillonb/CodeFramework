namespace System
{
    public static class DateTimeExtensions
    {
        public static string ToDaysAgo(this DateTime d)
        {
            var dt = DateTime.Now.Subtract(d.ToLocalTime());
            if (dt.TotalDays >= 365)
            {
                var years = Convert.ToInt32(dt.TotalDays / 365); 
                return years + (years > 1 ? " years ago".t() : " year ago".t());
            }
            if (dt.TotalDays >= 30)
            {
                var months = Convert.ToInt32(dt.TotalDays / 30); 
                return months + (months > 1 ? " months ago".t() : " month ago".t());
            }
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

