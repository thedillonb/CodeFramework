using System;
using CodeFramework.Core.ViewModels.Application;
using MonoTouch.Dialog.Utilities;
using MonoTouch.UIKit;
using ReactiveUI;
using Xamarin.Utilities.Images;
using Xamarin.Utilities.ViewControllers;

namespace CodeFramework.iOS.Views.Application
{
    public class StartupView : ViewModelViewController<StartupViewModel>, IImageUpdated
    {
        const float ImageSize = 128f;

        private UIImageView _imgView;
        private UILabel _statusLabel;
        private UIActivityIndicatorView _activityView;
        private UIStatusBarStyle _previousStatusbarStyle;

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            _imgView.Frame = new System.Drawing.RectangleF(View.Bounds.Width / 2 - ImageSize / 2, View.Bounds.Height / 2 - ImageSize / 2 - 30f, ImageSize, ImageSize);
            _statusLabel.Frame = new System.Drawing.RectangleF(0, _imgView.Frame.Bottom + 10f, View.Bounds.Width, 15f);
            _activityView.Center = new System.Drawing.PointF(View.Bounds.Width / 2, _statusLabel.Frame.Bottom + 16f + 16F);

            try
            {
                View.BackgroundColor = BackgroundHelper.CreateRepeatingBackground();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to produce background color for StartupView: {0}", e.Message);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutosizesSubviews = true;

            _imgView = new UIImageView();
            _imgView.Layer.CornerRadius = ImageSize / 2;
            _imgView.Layer.MasksToBounds = true;
            Add(_imgView);

            Add(_statusLabel = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.FromName("HelveticaNeue", 13f),
                TextColor = UIColor.FromWhiteAlpha(0.34f, 1f)
            });

            Add(_activityView = new UIActivityIndicatorView
            {
                HidesWhenStopped = true,
                Color = UIColor.FromRGB(0.33f, 0.33f, 0.33f)
            });

            ViewModel.WhenAnyValue(x => x.IsLoggingIn).Subscribe(x =>
            {
                if (x)
                    _activityView.StartAnimating();
                else
                    _activityView.StopAnimating();
            });

            ViewModel.WhenAnyValue(x => x.ImageUrl).Subscribe(UpdatedImage);
            ViewModel.WhenAnyValue(x => x.Status).Subscribe(x => _statusLabel.Text = x);
        }

        public void UpdatedImage(Uri uri)
        {
            if (uri == null)
            {
                AssignUnknownUserImage();
            }
            else
            {
                var img = ImageLoader.DefaultRequestImage(uri, this);
                if (img == null)
                {
                    AssignUnknownUserImage();
                }
                else
                {
                    UIView.Transition(_imgView, 0.50f, UIViewAnimationOptions.TransitionCrossDissolve, () => _imgView.Image = img, null);
                }
            }
        }

        private void AssignUnknownUserImage()
        {
            var img = Images.LoginUserUnknown.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            _imgView.Image = img;
            _imgView.TintColor = UIColor.FromWhiteAlpha(0.34f, 1f);
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _previousStatusbarStyle = UIApplication.SharedApplication.StatusBarStyle;
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, false);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UIApplication.SharedApplication.SetStatusBarStyle(_previousStatusbarStyle, true);
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.Default;
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

