using System.Linq;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.Data;
using CodeFramework.Core.Services;

namespace CodeFramework.Core.ViewModels
{
    public class StartupViewModel : BaseViewModel
    {
        public ICommand StartupCommand
        {
            get { return new MvxCommand(Startup);}
        }

        /// <summary>
        /// Execute startup code
        /// </summary>
        protected virtual void Startup()
        {
            Login();
        }

        /// <summary>
        /// Gets the default account. If there is not one assigned it will pick the first in the account list.
        /// If there isn't one, it'll just return null.
        /// </summary>
        /// <returns>The default account.</returns>
        protected IAccount GetDefaultAccount()
        {
            var accounts = Mvx.Resolve<IAccountsService<IAccount>>();
            return accounts.GetDefault();
        }

        private void Login()
        {
            var defaultAccount = GetDefaultAccount();
            if (defaultAccount == null)
            {
                this.ShowViewModel<AccountsViewModel>();
                return;
            }




//            var defaultAccount = GetDefaultAccount();
//
//            //There's no accounts... or something bad has happened to the default
//            if (Application.Accounts.Count == 0 || defaultAccount == null)
//            {
//                var login = new AccountTypeViewController();
//                login.NavigationItem.LeftBarButtonItem = null;
//                var navCtrl = new CustomNavigationController(this, login);
//                Transitions.TransitionToController(navCtrl);
//                return;
//            }
//
//            //Don't remember, prompt for password
//            if (defaultAccount.DontRemember)
//            {
//                ShowAccountsAndSelectedUser(defaultAccount);
//            }
//            //If the user wanted to remember the account
//            else
//            {
//                try
//                {
//                    await Utils.Login.LoginAccount(defaultAccount, this);
//                }
//                catch (Exception e)
//                {
//                    //Wow, what a surprise that there's issues using await and a catch here...
//                    MonoTouch.Utilities.ShowAlert("Error".t(), e.Message, () => ShowAccountsAndSelectedUser(defaultAccount));
//                }
//            }
        }

        private void ShowAccountsAndSelectedUser(IAccount account)
        {
//            var accountsController = new AccountsViewController();
//            accountsController.NavigationItem.LeftBarButtonItem = null;
//            var login = new LoginViewController(account);
//
//            var navigationController = new CustomNavigationController(this, accountsController);
//            navigationController.PushViewController(login, false);
//            Transitions.TransitionToController(navigationController);
        }
    }
}
