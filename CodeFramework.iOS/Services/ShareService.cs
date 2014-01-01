using System;
using CodeFramework.Core.Services;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cirrious.CrossCore.Touch.Views;
using Cirrious.CrossCore;

namespace CodeFramework.iOS.Services
{
	public class ShareService : IShareService
    {
		private readonly IMvxTouchModalHost _modalHost;

		public ShareService()
		{
			_modalHost = Mvx.Resolve<IMvxTouchModalHost>();
		}

		public void ShareUrl(string url)
		{
			var item = new NSUrl(url);
			var activityItems = new NSObject[] { item };
			UIActivity[] applicationActivities = null;
			var activityController = new UIActivityViewController (activityItems, applicationActivities);
			_modalHost.PresentModalViewController(activityController, true);
		}
    }
}

