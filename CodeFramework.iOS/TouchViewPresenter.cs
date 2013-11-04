using System;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.iOS.ViewControllers;
using CodeFramework.iOS.Views;
using CodeFramework.Utils;
using MonoTouch.SlideoutNavigation;
using MonoTouch.UIKit;

namespace CodeFramework.iOS
{
    public class TouchViewPresenter : MvxBaseTouchViewPresenter
    {
        private readonly UIWindow _window;
        private SlideoutNavigationController _slidoutController;

        public TouchViewPresenter(UIWindow window)
        {
            _window = window;
        }

        public override void Show(MvxViewModelRequest request)
        {
            var view = Mvx.Resolve<IMvxTouchViewCreator>().CreateView(request);
            var uiView = view as UIViewController;

            if (uiView == null)
                throw new InvalidOperationException("Asking to show a view which is not a UIViewController!");

            if (uiView is StartupView)
            {
                _window.RootViewController = uiView;
            }
            else if (uiView is AccountsView)
            {
                Transitions.Transition(_window, uiView, UIViewAnimationOptions.TransitionFlipFromRight);
            }
            else
            {
                _slidoutController.TopView.NavigationController.PushViewController(uiView, true);
            }
        }

        public void GotoSlideout(SlideoutNavigationController slidoutController, UIViewController menuViewController)
        {
            _slidoutController = slidoutController;
            _slidoutController.MenuView = menuViewController;
            Transitions.Transition(_window, _slidoutController, UIViewAnimationOptions.TransitionFlipFromRight);
        }
    }
}
