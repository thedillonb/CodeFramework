using System;
using System.Threading;
using System.Threading.Tasks;
using MBProgressHUD;
using MonoTouch;
using MonoTouch.UIKit;

namespace CodeFramework.iOS.Utils
{
    public static class ViewControllerExtensions
    {
		public static IHud CreateHud(this UIViewController controller)
		{
			return new Hud(controller.View);
		}

		public async static Task<T> DoWorkAsync<T>(this UIViewController controller, string workTitle, Func<Task<T>> work)
		{
			var parent = controller.View;
			var hud = new MTMBProgressHUD(parent) {
				Mode = MBProgressHUDMode.Indeterminate, 
				LabelText = workTitle,
				RemoveFromSuperViewOnHide = true,
				AnimationType = MBProgressHUDAnimation.MBProgressHUDAnimationFade
			};
			parent.AddSubview(hud);
			hud.Show(true);

			//Make sure the Toolbar is disabled too
			if (controller.ToolbarItems != null)
			{
				foreach (var t in controller.ToolbarItems)
					t.Enabled = false;
			}

			try
			{
				return await DoWorkNoHudAsync(controller, work);
			}
			finally
			{
				hud.Hide(true);

				//Enable all the toolbar items
				if (controller.ToolbarItems != null)
				{
					foreach (var t in controller.ToolbarItems)
						t.Enabled = true;
				}
			}
		}


        public async static Task DoWorkAsync(this UIViewController controller, string workTitle, Func<Task> work)
        {
            UIView parent = null;

            //Don't attach it to the UI window. It doesn't work well with orientation
//            if (controller.View.Superview is UIWindow)
                parent = controller.View;
//            else
//                parent = controller.View.Superview;

            var hud = new MTMBProgressHUD(parent) {
                Mode = MBProgressHUDMode.Indeterminate, 
				LabelText = workTitle,
				RemoveFromSuperViewOnHide = true,
				AnimationType = MBProgressHUDAnimation.MBProgressHUDAnimationFade
            };
            parent.AddSubview(hud);
            hud.Show(true);

            //Make sure the Toolbar is disabled too
            if (controller.ToolbarItems != null)
            {
                foreach (var t in controller.ToolbarItems)
                    t.Enabled = false;
            }

            try
            {
                await DoWorkNoHudAsync(controller, work);
            }
            finally
            {
                hud.Hide(true);

                //Enable all the toolbar items
                if (controller.ToolbarItems != null)
                {
                    foreach (var t in controller.ToolbarItems)
                        t.Enabled = true;
                }
            }
        }

		public async static Task<T> DoWorkNoHudAsync<T>(this UIViewController controller, Func<Task<T>> work)
		{
			try
			{
				Utilities.PushNetworkActive();
				return await work();
			}
			catch (Exception e)
			{
				Utilities.LogException(e.Message, e);
				throw e;
			}
			finally 
			{
				Utilities.PopNetworkActive();
			}
		}

        public async static Task DoWorkNoHudAsync(this UIViewController controller, Func<Task> work)
        {
            try
            {
                Utilities.PushNetworkActive();
                await work();
            }
            catch (Exception e)
            {
                Utilities.LogException(e.Message, e);
                throw e;
            }
            finally 
            {
                Utilities.PopNetworkActive();
            }
        }

        public static void DoWorkNoHud(this UIViewController controller, Action work, Action<Exception> error = null, Action final = null)
        {
            ThreadPool.QueueUserWorkItem(delegate {
                try
                {
                    Utilities.PushNetworkActive();
                    work();
                }
                catch (Exception e)
                {
                    Utilities.LogException(e.Message, e);
                    if (error != null)
                        controller.InvokeOnMainThread(() => error(e));
                }
                finally 
                {
                    Utilities.PopNetworkActive();
                    if (final != null)
                        controller.InvokeOnMainThread(() => final());
                }
            });
        }
    }
}

