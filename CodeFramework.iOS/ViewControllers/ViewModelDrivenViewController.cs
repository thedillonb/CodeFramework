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
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Bindings;
using System.Collections.Generic;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.CrossCore.Touch.Views;

namespace CodeFramework.iOS.ViewControllers
{
	public abstract class ViewModelDrivenViewController : BaseDialogViewController, IMvxTouchView, IMvxEventSourceViewController
    {
        protected ErrorView CurrentError;
        private bool _firstSeen;
        private MBProgressHUD.MTMBProgressHUD _loadingHud;
		
        public override void ViewDidLoad()
        {

            if (ViewModel is ILoadableViewModel)
                RefreshRequested += HandleRefreshRequested;

            base.ViewDidLoad();
            ViewDidLoadCalled.Raise(this);
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
            var loadableViewModel = ViewModel as ILoadableViewModel;
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
                finally
                {
                    ReloadComplete();
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

                var loadableViewModel = ViewModel as ILoadableViewModel;
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

                    // Indicate that the reload is complete
                    ReloadComplete();
                }
            }

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

