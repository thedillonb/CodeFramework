using System;
using MonoTouch.UIKit;
using System.Linq;
using CodeFramework.Controllers;
using CodeFramework.Utils;

namespace CodeFramework.Controllers
{
    public abstract class StartupController : UIViewController
    {
        private UIImageView _imgView;
        private UIImage _img;

        public StartupController()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                WantsFullScreenLayout = true;
        }

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
                    _img = UIImageHelper.FromFileAuto(MonoTouch.Utilities.IsTall ? "Default-568h" : "Default");
                }
                else
                {
                    if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.PortraitUpsideDown)
                        _img = UIImageHelper.FromFileAuto("Default-Portrait");
                    else
                        _img = UIImageHelper.FromFileAuto("Default-Landscape");
                }

                if (_img != null)
                    _imgView.Image = _img;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to show StartupController: " + e.Message);
                GoogleAnalytics.GAI.SharedInstance.DefaultTracker.TrackException(false, e.Message);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutosizesSubviews = true;
            _imgView = new UIImageView();
            _imgView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            Add(_imgView);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            Analytics.Tracker.TrackView(this.GetType().Name);

            //Start the login
            ProcessAccounts();
        }

		protected abstract void ProcessAccounts ();

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

        [Obsolete]
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                if (toInterfaceOrientation == UIInterfaceOrientation.Portrait || toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown)
                    return true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// A custom navigation controller specifically for iOS6 that locks the orientations to what the StartupControler's is.
        /// </summary>
        protected class CustomNavigationController : UINavigationController
        {
            readonly StartupController _parent;
            public CustomNavigationController(StartupController parent, UIViewController root) : base(root) 
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

