using System;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch;
using MonoTouch.UIKit;
using CodeFramework.Core.ViewModels;
using CodeFramework.iOS.Utils;
using Cirrious.MvvmCross.Plugins.Messenger;
using CodeFramework.iOS.ViewControllers;

namespace CodeFramework.iOS.Views
{
	public class StartupView : ViewModelDrivenDialogViewController
    {
        private UIImageView _imgView;
        private UIImage _img;
		private IHud _hud;

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
			_hud = new Hud(View);
            Add(_imgView);

			var vm = (BaseStartupViewModel)ViewModel;
			vm.Bind(x => x.IsLoggingIn, x =>
				{
					if (x)
					{
						_hud.Show("Logging in...");
					}
					else
					{
						_hud.Hide();
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

