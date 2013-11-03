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

        public AccountsViewModel(IAccountsService<IAccount> accountsService)
        {
            _accountsService = accountsService;
        }

        public void Init()
        {
            _accounts.Reset(_accountsService);
        }
    }
}
