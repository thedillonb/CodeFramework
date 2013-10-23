using CodeFramework.Views;
using MonoTouch;
using System.Threading.Tasks;
using System;
using CodeFramework.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CodeFramework.ViewControllers
{
    public abstract class ViewModelDrivenViewController : BaseDialogViewController
    {
        protected ErrorView CurrentError;
        private bool _firstSeen;

        public ViewModel ViewModel { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
		protected ViewModelDrivenViewController(bool push = true, bool refresh = true)
            : base(push)
        {
            if (refresh)
                RefreshRequested += HandleRefreshRequested;
        }

        private async void HandleRefreshRequested(object sender, EventArgs e)
        {
            var loadableViewModel = ViewModel as ILoadableViewModel;
            if (loadableViewModel != null)
            {
                try
                {
                    await this.DoWorkNoHudAsync(() => loadableViewModel.Load(true));
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

        public override async void ViewWillAppear(bool animated)
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
                        //Do some work
                        await this.DoWorkTest("Loading...", () => loadableViewModel.Load(false));
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
    }
}

