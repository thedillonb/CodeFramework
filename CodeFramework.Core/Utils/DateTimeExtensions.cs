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
                return years + (years > 1 ? " years ago" : " year ago");
            }
            if (dt.TotalDays >= 30)
            {
                var months = Convert.ToInt32(dt.TotalDays / 30); 
                return months + (months > 1 ? " months ago" : " month ago");
            }
            if (dt.TotalDays > 1)
                return Convert.ToInt32(dt.TotalDays) + " days ago";
            if (dt.TotalHours > 1)
                return Convert.ToInt32(dt.TotalHours) + " hours ago";
            if (dt.TotalMinutes > 1)
                return Convert.ToInt32(dt.TotalMinutes) + " minutes ago";
            return "moments ago";
        }

		public static string ToDaysAgo(this DateTimeOffset d)
		{
			var dt = DateTimeOffset.Now.Subtract(d);
			if (dt.TotalDays >= 365)
			{
				var years = Convert.ToInt32(dt.TotalDays / 365); 
				return years + (years > 1 ? " years ago" : " year ago");
			}
			if (dt.TotalDays >= 30)
			{
				var months = Convert.ToInt32(dt.TotalDays / 30); 
				return months + (months > 1 ? " months ago" : " month ago");
			}
			if (dt.TotalDays > 1)
				return Convert.ToInt32(dt.TotalDays) + " days ago";
			if (dt.TotalHours > 1)
				return Convert.ToInt32(dt.TotalHours) + " hours ago";
			if (dt.TotalMinutes > 1)
				return Convert.ToInt32(dt.TotalMinutes) + " minutes ago";
			return "moments ago";
		}

        public static int TotalDaysAgo(this DateTime d)
        {
            return Convert.ToInt32(Math.Round(DateTime.Now.Subtract(d.ToLocalTime()).TotalDays));
        }

		public static int TotalDaysAgo(this DateTimeOffset d)
		{
			return Convert.ToInt32(Math.Round(DateTimeOffset.Now.Subtract(d).TotalDays));
		}
    }
}

