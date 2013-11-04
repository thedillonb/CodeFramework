using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.Data;
using CodeFramework.Core.Services;
using CodeFramework.Core.Utils;

namespace CodeFramework.Core.ViewModels
{
    public class AccountsViewModel : BaseViewModel
    {
        private readonly CustomObservableCollection<IAccount> _accounts = new CustomObservableCollection<IAccount>();
        private readonly IAccountsService<IAccount> _accountsService;

        public CustomObservableCollection<IAccount> Accounts
        {
            get { return _accounts; }
        }

        public ICommand AddAccountCommand
        {
            get { return new MvxCommand(AddAccount); }
        }

        public ICommand SelectAccountCommand
        {
            get { return new MvxCommand<IAccount>(SelectAccount); }
        }
//
//        public AccountsViewModel(IAccountsService<IAccount> accountsService)
//        {
//            _accountsService = accountsService;
//        }

        private void AddAccount()
        {
            
        }

        private void SelectAccount(IAccount account)
        {
            
        }

        public void Init()
        {
            //_accounts.Reset(_accountsService);
        }
    }
}
