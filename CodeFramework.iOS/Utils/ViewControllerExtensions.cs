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
        public static void DoWork(this UIViewController controller, string workTitle, Action work, Action<Exception> error = null, Action final = null)
        {
            UIView parent = null;

            //Don't attach it to the UI window. It doesn't work well with orientation
//            if (controller.View.Superview is UIWindow)
                parent = controller.View;
//            else
//                parent = controller.View.Superview;

            var hud = new MTMBProgressHUD(parent) {
                Mode = MBProgressHUDMode.Indeterminate, 
                LabelText = workTitle
            };
            parent.AddSubview(hud);
            hud.Show(true);

            //Make sure the Toolbar is disabled too
            if (controller.ToolbarItems != null)
            {
                foreach (var t in controller.ToolbarItems)
                    t.Enabled = false;
            }

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
                
                if (hud != null)
                {
                    controller.InvokeOnMainThread(delegate {
                        hud.Hide(true);
                        hud.RemoveFromSuperview();

                        //Enable all the toolbar items
                        if (controller.ToolbarItems != null)
                        {
                            foreach (var t in controller.ToolbarItems)
                                t.Enabled = true;
                        }
                    });
                }
            });
        }

        public async static Task DoWorkTest(this UIViewController controller, string workTitle, Func<Task> work)
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
//            hud.DidHide += (sender, e) => { 
//                hud.RemoveFromSuperview();
//                hud.Dispose();
//            };

            parent.AddSubview(hud);
            hud.Show(true);

            //Make sure the Toolbar is disabled too
            if (controller.ToolbarItems != null)
                foreach (var t in controller.ToolbarItems)
                    t.Enabled = false;

            try
            {
                Utilities.PushNetworkActive();
                await work();
            }
            catch (Exception e)
            {
                Utilities.LogException(e.Message, e);
                throw;
            }
            finally 
            {
                hud.Hide(true);
                Utilities.PopNetworkActive();

                //Enable all the toolbar items
                if (controller.ToolbarItems != null)
                    foreach (var t in controller.ToolbarItems)
                        t.Enabled = true;
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

        public static void DoWork(this UIViewController controller, Action work, Action<Exception> error = null, Action final = null)
        {
            controller.DoWork("Loading...".t(), work, error, final);
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

