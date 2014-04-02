using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.Data;
using CodeFramework.Core.Services;

namespace CodeFramework.Core.ViewModels
{
    public abstract class BaseStartupViewModel : BaseViewModel
    {
        private bool _isLoggingIn;

        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            protected set
            {
                _isLoggingIn = value;
                RaisePropertyChanged(() => IsLoggingIn);
            }
        }

        public ICommand StartupCommand
        {
            get { return new MvxCommand(Startup);}
        }

        /// <summary>
        /// Execute startup code
        /// </summary>
        protected abstract void Startup();

        /// <summary>
        /// Gets the default account. If there is not one assigned it will pick the first in the account list.
        /// If there isn't one, it'll just return null.
        /// </summary>
        /// <returns>The default account.</returns>
        protected IAccount GetDefaultAccount()
        {
            var accounts = GetService<IAccountsService>();
            return accounts.GetDefault();
        }
    }
}
