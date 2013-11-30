using System;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core;
using CodeFramework.iOS.ViewControllers;
using CodeFramework.iOS.Views;
using CodeFramework.Utils;
using MonoTouch.UIKit;

namespace CodeFramework.iOS
{
    public class TouchViewPresenter : MvxBaseTouchViewPresenter
    {
        private readonly UIWindow _window;
        private UINavigationController _generalNavigationController;
        private SlideoutNavigationViewController _slideoutController;

        public TouchViewPresenter(UIWindow window)
        {
            _window = window;
        }

        public override void Show(MvxViewModelRequest request)
        {
            var viewCreator = Mvx.Resolve<IMvxTouchViewCreator>();
            var view = viewCreator.CreateView(request);
            var uiView = view as UIViewController;

            if (uiView == null)
                throw new InvalidOperationException("Asking to show a view which is not a UIViewController!");

            if (uiView is StartupView)
            {
                _window.RootViewController = uiView;
            }
            else if (uiView is AccountsView)
            {
                _slideoutController = null;
                _generalNavigationController = new UINavigationController(uiView);
//				_generalNavigationController.NavigationBar.BarTintColor = Theme.CurrentTheme.AccountsNavigationBarTint;
				_generalNavigationController.NavigationBar.Translucent = false;

                Transitions.Transition(_window, _generalNavigationController, UIViewAnimationOptions.TransitionFlipFromRight);
            }
            else if (uiView is MenuBaseViewController)
            {
                _slideoutController = new SlideoutNavigationViewController();
				_slideoutController.MenuViewLeft = uiView;
//				uiView.NavigationController.NavigationBar.BarTintColor = Theme.CurrentTheme.SlideoutNavigationBarTint;
				uiView.NavigationController.NavigationBar.Translucent = false;

                Transitions.Transition(_window, _slideoutController, UIViewAnimationOptions.TransitionFlipFromRight);
            }
            else
            {
                if (request.PresentationValues != null && request.PresentationValues.ContainsKey(PresentationValues.SlideoutRootPresentation))
                {
                    _slideoutController.SelectView(uiView);
                    _generalNavigationController = _slideoutController.TopView.NavigationController;
//					_generalNavigationController.NavigationBar.BarTintColor = Theme.CurrentTheme.ApplicationNavigationBarTint;
					_generalNavigationController.NavigationBar.Translucent = false;
                }
                else
                {
                    _generalNavigationController.PushViewController(uiView, true);
                }
            }
        }
    }
}
