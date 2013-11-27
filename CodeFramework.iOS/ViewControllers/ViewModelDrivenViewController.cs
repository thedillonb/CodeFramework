using System;
using System.Collections.Generic;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Bindings;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.ViewModels;
using CodeFramework.iOS.Views;
using CodeFramework.ViewControllers;
using MonoTouch;
using MonoTouch.UIKit;

namespace CodeFramework.iOS.ViewControllers
{
	public abstract class ViewModelDrivenViewController : BaseDialogViewController, IMvxTouchView, IMvxEventSourceViewController
    {
        protected ErrorView CurrentError;
        private bool _firstSeen;
        private MBProgressHUD.MTMBProgressHUD _loadingHud;
		private UIRefreshControl _refreshControl;
		private float _beforeLoadingOffset;
		
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			ViewDidLoadCalled.Raise(this);

			var loadableViewModel = ViewModel as LoadableViewModel;
			if (loadableViewModel != null)
			{
				_refreshControl = new UIRefreshControl();
				RefreshControl = _refreshControl;
				_refreshControl.ValueChanged += HandleRefreshRequested;
				loadableViewModel.Bind(x => x.IsLoading, x =>
				{
						if (x)
						{
							_beforeLoadingOffset = TableView.ContentInset.Top;
							_refreshControl.BeginRefreshing();
							TableView.SetContentOffset(new System.Drawing.PointF(0, -TableView.ContentInset.Top), true);
						}
						else
						{
							TableView.SetContentOffset(new System.Drawing.PointF(0, _beforeLoadingOffset), true);
							_refreshControl.EndRefreshing();
						}
				});
			}
        }

        private void StartLoading()
        {
            if (_loadingHud != null)
                return;

            //Make sure the Toolbar is disabled too
            if (this.ToolbarItems != null)
            {
                foreach (var t in this.ToolbarItems)
                    t.Enabled = false;
            }

            _loadingHud = new MBProgressHUD.MTMBProgressHUD(this.View) {
                Mode = MBProgressHUD.MBProgressHUDMode.Indeterminate, 
                LabelText = "Loading..."
            };

            this.View.AddSubview(_loadingHud);
            _loadingHud.Show(true);
        }

        private void EndLoading()
        {
            if (_loadingHud == null)
                return;

            _loadingHud.Hide(true);
            _loadingHud.RemoveFromSuperview();

            //Enable all the toolbar items
            if (this.ToolbarItems != null)
            {
                foreach (var t in this.ToolbarItems)
                    t.Enabled = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
		protected ViewModelDrivenViewController(bool push = true)
            : base(push)
        {
			this.AdaptForBinding();
        }

        private void HandleRefreshRequested(object sender, EventArgs e)
        {
			var loadableViewModel = ViewModel as LoadableViewModel;
            if (loadableViewModel != null)
            {
                try
                {
                    loadableViewModel.LoadCommand.Execute(true);
                }
                catch (Exception ex)
                {
                    Utilities.ShowAlert("Error".t(), ex.Message);
                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //We only want to run this code once, when teh view is first seen...
            if (!_firstSeen)
            {
                _firstSeen = true;

				var loadableViewModel = ViewModel as LoadableViewModel;
                if (loadableViewModel != null)
                {
                    try
                    {
                        loadableViewModel.LoadCommand.Execute(false);
                    }
                    catch (Exception e)
                    {
                        CurrentError = ErrorView.Show(this.View, e.Message);
                    }
                }
            }

        }

		public override float GetHeightForFooter(MonoTouch.UIKit.UITableView tableView, int section)
		{
			if (tableView.Style == MonoTouch.UIKit.UITableViewStyle.Grouped)
				return 2;
			return base.GetHeightForFooter(tableView, section);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			ViewWillDisappearCalled.Raise(this, animated);
		}

		protected T Bind<T>(T element, string bindingDescription)
		{
			return element.Bind(this, bindingDescription);
		}

		protected T Bind<T>(T element, IEnumerable<MvxBindingDescription> bindingDescription)
		{
			return element.Bind(this, bindingDescription);
		}

		public object DataContext
		{
			get { return BindingContext.DataContext; }
			set { BindingContext.DataContext = value; }
		}

		public IMvxViewModel ViewModel
		{
			get { return DataContext as IMvxViewModel;  }
			set { DataContext = value; }
		}

		public IMvxBindingContext BindingContext { get; set; }

		public MvxViewModelRequest Request { get; set; }

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			ViewDidDisappearCalled.Raise(this, animated);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			ViewDidAppearCalled.Raise(this, animated);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeCalled.Raise(this);
			}
			base.Dispose(disposing);
		}

		public event EventHandler DisposeCalled;
		public event EventHandler ViewDidLoadCalled;
		public event EventHandler<MvxValueEventArgs<bool>> ViewWillAppearCalled;
		public event EventHandler<MvxValueEventArgs<bool>> ViewDidAppearCalled;
		public event EventHandler<MvxValueEventArgs<bool>> ViewDidDisappearCalled;
		public event EventHandler<MvxValueEventArgs<bool>> ViewWillDisappearCalled;
    }
}

