using System;
using GoogleAnalytics;

namespace CodeFramework.Utils
{
    public class Analytics
    {
        public static GAITracker Tracker
        {
            get { return GoogleAnalytics.GAI.SharedInstance.DefaultTracker; }
        }
    }
}

