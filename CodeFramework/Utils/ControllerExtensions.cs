using System;
using MonoTouch.UIKit;
using RedPlum;
using System.Threading;
using MonoTouch;

namespace CodeFramework.Controllers
{
    public static class ControllerExtensions
    {
        public static void DoWork(this UIViewController controller, string workTitle, Action work, Action<Exception> error = null, Action final = null)
        {
            MBProgressHUD hud = null;
            UIView parent = null;

            //Don't attach it to the UI window. It doesn't work well with orientation
            if (controller.View.Superview is UIWindow)
                parent = controller.View;
            else
                parent = controller.View.Superview;

            hud = new MBProgressHUD(parent) {Mode = MBProgressHUDMode.Indeterminate, TitleText = workTitle};
            parent.AddSubview(hud);
            hud.Show(true);

            ThreadPool.QueueUserWorkItem(delegate {
                try
                {
                    Utilities.PushNetworkActive();
                    work();
                }
                catch (Exception e)
                {
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
                    });
                }
            });
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

