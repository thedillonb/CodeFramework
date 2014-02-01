using System;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.iOS.ViewControllers;
using CodeFramework.iOS.Views;
using MonoTouch.UIKit;
using CodeFramework.Core;
using MonoTouch.SlideoutNavigation;

namespace CodeFramework.iOS
{
    public class TouchViewPresenter : MvxBaseTouchViewPresenter
    {
        private readonly UIWindow _window;
        private UINavigationController _generalNavigationController;
        private SlideoutNavigationController _slideoutController;

        public TouchViewPresenter(UIWindow window)
        {
            _window = window;
        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            var closeHint = hint as MvxClosePresentationHint;
            if (closeHint != null)
            {
                for (int i = _generalNavigationController.ViewControllers.Length - 1; i >= 1; i--)
                {
                    var vc = _generalNavigationController.ViewControllers[i];
                    var touchView = vc as IMvxTouchView;
                    if (touchView != null && touchView.ViewModel == closeHint.ViewModelToClose)
                    {
                        _generalNavigationController.PopToViewController(_generalNavigationController.ViewControllers[i - 1], true);
                        return;
                    }
                }

                //If it didnt trigger above it's because it was probably the root.
                _generalNavigationController.PopToRootViewController(true);
            }
        }

        public override void Show(MvxViewModelRequest request)
        {
            var viewCreator = Mvx.Resolve<IMvxTouchViewCreator>();
            var view = viewCreator.CreateView(request);
            var uiView = view as UIViewController;

            if (uiView == null)
                throw new InvalidOperationException("Asking to show a view which is not a UIViewController!");

            if (uiView is IMvxModalTouchView)
            {
                var modalNavigationController = new UINavigationController(uiView);
                modalNavigationController.NavigationBar.Translucent = false;
                modalNavigationController.Toolbar.Translucent = false;
                uiView.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Theme.CurrentTheme.CancelButton, UIBarButtonItemStyle.Plain, (s, e) => modalNavigationController.DismissViewController(true, null));
                PresentModalViewController(modalNavigationController, true);
            }
            else if (uiView is StartupView)
            {
                _window.RootViewController = uiView;
            }
            else if (uiView is AccountsView)
            {
                _slideoutController = null;
                _generalNavigationController = new UINavigationController(uiView);
                _generalNavigationController.NavigationBar.Translucent = false;
                _generalNavigationController.Toolbar.Translucent = false;

                Transition(_generalNavigationController, UIViewAnimationTransition.FlipFromRight);
            }
            else if (uiView is MenuBaseViewController)
            {
                _slideoutController = new SimpleSlideoutNavigationController();

                var menuNavController = new MenuNavigationController(uiView, _slideoutController);
                menuNavController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
                menuNavController.NavigationBar.ShadowImage = new UIImage();
                menuNavController.NavigationBar.Translucent = true;
                menuNavController.View.BackgroundColor = UIColor.Clear;
                _slideoutController.MenuViewController = menuNavController;


//                uiView.NavigationController.NavigationBar.Translucent = false;
//                uiView.NavigationController.Toolbar.Translucent = false;
//                uiView.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(50, 50, 50);
                Transition(_slideoutController, UIViewAnimationTransition.FlipFromLeft);
            }
            else
            {
                if (request.PresentationValues != null && request.PresentationValues.ContainsKey(PresentationValues.SlideoutRootPresentation))
                {
                    var mainNavigationController = new MainNavigationController(uiView, _slideoutController, new UIBarButtonItem(Theme.CurrentTheme.ThreeLinesButton, UIBarButtonItemStyle.Plain, (s, e) => _slideoutController.Open(true)));
                    _generalNavigationController = mainNavigationController;
                    _slideoutController.SetMainViewController(mainNavigationController, true);


                    //_generalNavigationController.NavigationBar.BarTintColor = Theme.CurrentTheme.ApplicationNavigationBarTint;
                    _generalNavigationController.NavigationBar.Translucent = false;
                    _generalNavigationController.Toolbar.Translucent = false;
                }
                else
                {
                    _generalNavigationController.PushViewController(uiView, true);
                }
            }
        }

        public override bool PresentModalViewController(UIViewController viewController, bool animated)
        {
            if (_window.RootViewController == null)
                return false;
            _window.RootViewController.PresentViewController(viewController, true, null);
            return true;
        }

        private void Transition(UIViewController controller, UIViewAnimationTransition animation)
        {
            UIView.BeginAnimations("view_presenter_transition");
            _window.RootViewController = controller;
            UIView.SetAnimationDuration(0.6);
            UIView.SetAnimationTransition(animation, _window, false);
            UIView.CommitAnimations();
        }
    }
}
