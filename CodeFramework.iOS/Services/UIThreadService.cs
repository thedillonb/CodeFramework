using System;
using CodeFramework.Core.Services;
using MonoTouch.Foundation;

namespace CodeFramework.iOS.Services
{
	public class UIThreadService : IUIThreadService
    {
		private static readonly NSObject _obj = new NSObject();

		public void MarshalOnUIThread(Action a)
		{
			_obj.InvokeOnMainThread(() => a());
		}
    }
}

