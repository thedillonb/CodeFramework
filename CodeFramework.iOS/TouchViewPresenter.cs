using System;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.iOS.ViewControllers;
using CodeFramework.iOS.Views;
using MonoTouch.UIKit;
using CodeFramework.Core;

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

		public override void ChangePresentation(MvxPresentationHint hint)
		{
			if (hint is MvxClosePresentationHint)
			{
				_generalNavigationController.PopViewControllerAnimated(true);
			}
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

				Transition(_generalNavigationController, UIViewAnimationTransition.FlipFromRight);
            }
			else if (uiView is MenuBaseViewController)
            {
                _slideoutController = new SlideoutNavigationViewController();
				_slideoutController.MenuViewLeft = uiView;
				uiView.NavigationController.NavigationBar.Translucent = false;
				Transition(_slideoutController, UIViewAnimationTransition.FlipFromLeft);
            }
            else
            {
                if (request.PresentationValues != null && request.PresentationValues.ContainsKey(PresentationValues.SlideoutRootPresentation))
                {
                    _slideoutController.SelectView(uiView);
                    _generalNavigationController = _slideoutController.TopView.NavigationController;
					//_generalNavigationController.NavigationBar.BarTintColor = Theme.CurrentTheme.ApplicationNavigationBarTint;
					_generalNavigationController.NavigationBar.Translucent = false;
				}
                else
                {
                    _generalNavigationController.PushViewController(uiView, true);
                }
            }
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
