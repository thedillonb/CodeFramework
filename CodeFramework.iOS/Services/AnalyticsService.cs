using System;
using CodeFramework.Core.Services;
using MonoTouch.Foundation;
using GoogleAnalytics.iOS;

namespace CodeFramework.iOS.Services
{
	public class AnalyticsService : IAnalyticsService
    {
		IGAITracker _tracker;

		public void Init(string tracker, string name)
		{
			var result = GoogleAnalytics.iOS.GAI.SharedInstance;
			_tracker = result.GetTracker(name, tracker);
			result.TrackUncaughtExceptions = true;
			result.DispatchInterval = 30;
		}

		public bool Enabled
		{
			get { return !GoogleAnalytics.iOS.GAI.SharedInstance.OptOut; }
			set { GoogleAnalytics.iOS.GAI.SharedInstance.OptOut = !value; }
		}

		public void LogException(Exception e)
		{
			//_tracker..TrackException(false, e.Message + " - " + e.StackTrace);
		}
    }
}

