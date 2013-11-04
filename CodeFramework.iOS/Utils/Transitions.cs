using System;
using MonoTouch.UIKit;

namespace CodeFramework.Utils
{
    /// <summary>
    /// Just some silly transition code that I didn't want to write a hundred times over and over again.
    /// </summary>
    public static class Transitions
    {
        public static void TransitionToController(UIWindow window, UIViewController controller, Action doneCallback = null)
        {
            Transition(window, controller, UIViewAnimationOptions.TransitionCrossDissolve, 1.0, doneCallback);
        }

        public static void Transition(UIWindow window, UIViewController controller, UIViewAnimationOptions options, double duration = 0.6, Action doneCallback = null)
        {
            UIView.Transition(window, duration, options, () => {
                var oldState = UIView.AnimationsEnabled;
                UIView.AnimationsEnabled = false;
                window.RootViewController = controller;
                UIView.AnimationsEnabled = oldState;
            }, () => {
                if (doneCallback != null)
                    doneCallback();
            });
        }
    }
}

