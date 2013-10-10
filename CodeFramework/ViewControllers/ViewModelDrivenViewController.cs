using CodeFramework.Views;
using MonoTouch;
using System.Threading.Tasks;
using System;
using CodeFramework.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CodeFramework.Controllers
{
    public abstract class ViewModelDrivenViewController : BaseDialogViewController
    {
        protected ErrorView CurrentError;
        private bool _firstSeen;

        public ViewModelBase ViewModel { get; protected set; }

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

        protected void Bind<T, R>(T viewModel, System.Linq.Expressions.Expression<Func<T, R>> outExpr, Action b) where T : CodeFramework.ViewModels.ViewModelBase
        {
            INotifyPropertyChanged m = viewModel;
            var expr = (System.Linq.Expressions.MemberExpression) outExpr.Body;
            var prop = (System.Reflection.PropertyInfo) expr.Member;
            var name = prop.Name;
            m.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                if (e.PropertyName.Equals(name))
                    BeginInvokeOnMainThread(new MonoTouch.Foundation.NSAction(b));
            };
        }

        protected void BindCollection<T, R>(T viewModel, System.Linq.Expressions.Expression<Func<T, R>> outExpr, Action<NotifyCollectionChangedEventArgs> b) where T : CodeFramework.ViewModels.ViewModelBase where R : INotifyCollectionChanged
        {
            var exp = outExpr.Compile();
            INotifyCollectionChanged m = exp(viewModel);
            m.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
                BeginInvokeOnMainThread(() => b(e));

            };
        }

        private void HandleRefreshRequested(object sender, EventArgs e)
        {
//            try
//            {
//                //Do some work
//                this.DoWorkTest("Loading...", () => ViewModel.Load(true));
//            }
//            catch (Exception e)
//            {
//                CurrentError = ErrorView.Show(this, e.Message);
//            }

            // Indicate that the reload is complete
            ReloadComplete();
        }

        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //We only want to run this code once, when teh view is first seen...
            if (!_firstSeen)
            {
                _firstSeen = true;
 
                try
                {
                    //Do some work
                    await this.DoWorkTest("Loading...", () => ViewModel.Load(false));
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

