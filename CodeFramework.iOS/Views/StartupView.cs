using System;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch;
using MonoTouch.UIKit;
using CodeFramework.Core.ViewModels;
using MBProgressHUD;

namespace CodeFramework.iOS.Views
{
    public class StartupView : MvxViewController
    {
        private UIImageView _imgView;
        private UIImage _img;
		private MTMBProgressHUD _hud;

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            try
            {
                if (_imgView != null)
                    _imgView.Frame = this.View.Bounds;

                if (_img != null)
                    _img.Dispose();
                _img = null;

                //Load the background image
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                {
                    _img = UIImageHelper.FromFileAuto(Utilities.IsTall ? "Default-568h" : "Default");
                }
                else
                {
                    if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.PortraitUpsideDown)
                        _img = UIImageHelper.FromFileAuto("Default-Portrait");
                    else
                        _img = UIImageHelper.FromFileAuto("Default-Landscape");
                }

                if (_img != null && _imgView != null)
                    _imgView.Image = _img;
            }
            catch (Exception e)
            {
                Utilities.LogException("Unable to show StartupController", e);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutosizesSubviews = true;
            _imgView = new UIImageView {AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight};
            Add(_imgView);

			_hud = new MTMBProgressHUD(View) {
				Mode = MBProgressHUDMode.Indeterminate, 
				LabelText = "Logging in...",
				RemoveFromSuperViewOnHide = true,
				AnimationType = MBProgressHUDAnimation.MBProgressHUDAnimationFade
			};

			var vm = (BaseStartupViewModel)ViewModel;
			vm.Bind(x => x.IsLoggingIn, x =>
				{
					if (x)
					{
						View.AddSubview(_hud);
						_hud.Show(true);
					}
					else
					{
						_hud.Hide(true);
					}
				});
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			var vm = (BaseStartupViewModel)ViewModel;
			vm.StartupCommand.Execute(null);
		}

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
            return UIInterfaceOrientationMask.All;
        }

        /// <summary>
        /// A custom navigation controller specifically for iOS6 that locks the orientations to what the StartupControler's is.
        /// </summary>
        protected class CustomNavigationController : UINavigationController
        {
            readonly StartupView _parent;
            public CustomNavigationController(StartupView parent, UIViewController root) : base(root) 
            { 
                _parent = parent;
            }

            public override bool ShouldAutorotate()
            {
                return _parent.ShouldAutorotate();
            }

            public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
            {
                return _parent.GetSupportedInterfaceOrientations();
            }
        }
    }
}

