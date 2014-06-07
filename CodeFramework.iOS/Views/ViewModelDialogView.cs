using System;
using System.Reactive.Linq;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using ReactiveUI;
using Xamarin.Utilities.Core.Services;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeFramework.iOS.Views
{
    public abstract class ViewModelDialogView<TViewModel> : DialogViewController, IViewFor<TViewModel> where TViewModel : ReactiveObject
    {
        protected readonly INetworkActivityService NetworkActivityService = IoC.Resolve<INetworkActivityService>();
        private UIRefreshControl _refreshControl;
        private bool _loaded;
        private bool _manualRefreshRequested;


        public TViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }

        protected ViewModelDialogView(UITableViewStyle style = UITableViewStyle.Plain)
            : base(style, null, true)
        {
            SearchPlaceholder = "Search";
        }

        public override void ViewDidLoad()
        {
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = string.Empty };

            base.ViewDidLoad();

            var loadableViewModel = ViewModel as LoadableViewModel;
            if (loadableViewModel != null)
            {
                _refreshControl = new UIRefreshControl();
                RefreshControl = _refreshControl;
                _refreshControl.ValueChanged += (s, e) =>
                {
                    _manualRefreshRequested = true;
                    loadableViewModel.LoadCommand.ExecuteIfCan();
                };
                loadableViewModel.LoadCommand.IsExecuting.Where(x => !x).Subscribe(x =>
                {
                    if (x)
                    {
                        _refreshControl.BeginRefreshing();

                        if (!_manualRefreshRequested)
                        {
                            UIView.Animate(0.25, 0f, UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.CurveEaseOut,
                                () => TableView.ContentOffset = new System.Drawing.PointF(0, -_refreshControl.Frame.Height), null);
                        }

                        if (ToolbarItems != null)
                        {
                            foreach (var t in ToolbarItems)
                                t.Enabled = false;
                        }
                    }
                    else
                    {
                        BeginInvokeOnMainThread(() =>
                        {
                            UIView.Animate(0.25, 0.0, UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.CurveEaseOut,
                                () => TableView.ContentOffset = new System.Drawing.PointF(0, 0), null);
                            _refreshControl.EndRefreshing();
                        });

                        if (ToolbarItems != null)
                        {
                            foreach (var t in ToolbarItems)
                                t.Enabled = true;
                        }

                        _manualRefreshRequested = false;
                    }
                });
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!_loaded)
            {
                _loaded = true;
                var loadableViewModel = ViewModel as LoadableViewModel;
                if (loadableViewModel != null)
                    loadableViewModel.LoadCommand.ExecuteIfCan();
            }
        }
    }
}

