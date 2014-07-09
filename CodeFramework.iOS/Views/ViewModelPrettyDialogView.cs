using System;
using CodeFramework.iOS.ViewComponents;
using MonoTouch.UIKit;
using CodeStash.iOS.Views;
using Xamarin.Utilities.Core.ViewModels;
using System.Reactive.Linq;

namespace CodeFramework.iOS.Views
{
    public class ViewModelPrettyDialogView<TViewModel> : ViewModelDialogView<TViewModel> where TViewModel : BaseViewModel
    {
        protected SlideUpTitleView SlideUpTitle;

        protected ImageAndTitleHeaderView HeaderView;

        public override string Title
        {
            get
            {
                return base.Title;
            }
            set
            {
                base.Title = value;
                if (HeaderView != null) HeaderView.Text = base.Title;
                if (SlideUpTitle != null) SlideUpTitle.Text = base.Title;
            }
        }

        protected override void Scrolled(System.Drawing.PointF point)
        {
            if (NavigationController != null &&
                NavigationController.NavigationItem != null)
            {
                if (point.Y > 0)
                {
                    NavigationController.NavigationBar.ShadowImage = null;
                }
                else
                {
                    if (NavigationController.NavigationBar.ShadowImage == null)
                        NavigationController.NavigationBar.ShadowImage = new UIImage();
                }
            }

            SlideUpTitle.Offset = 108 + 28f - point.Y;
        }

        public override void ViewWillAppear(bool animated)
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, animated);
            base.ViewWillAppear(animated);
            NavigationController.NavigationBar.ShadowImage = new UIImage();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.NavigationBar.ShadowImage = null;
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.TitleView = SlideUpTitle = new SlideUpTitleView(NavigationController.NavigationBar.Bounds.Height);
            SlideUpTitle.Offset = 100f;

            TableView.SectionHeaderHeight = 0;
            RefreshControl.TintColor = UIColor.LightGray;

            HeaderView = new ImageAndTitleHeaderView 
            { 
                BackgroundColor = NavigationController.NavigationBar.BackgroundColor,
                TextColor = UIColor.White,
                SubTextColor = UIColor.LightGray
            };

            var topBackgroundView = this.CreateTopBackground(HeaderView.BackgroundColor);
            var loadableViewModel = ViewModel as LoadableViewModel;
            if (loadableViewModel != null)
            {
                topBackgroundView.Hidden = true;
                loadableViewModel.LoadCommand.IsExecuting.Where(x => !x).Skip(1).Take(1).Subscribe(_ => topBackgroundView.Hidden = false);
            }
        }
    }
}

