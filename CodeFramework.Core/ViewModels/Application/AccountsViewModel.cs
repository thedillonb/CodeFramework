using System;
using System.Reactive.Linq;
using CodeFramework.Core.Data;
using CodeFramework.Core.Messages;
using CodeFramework.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeFramework.Core.ViewModels.Application
{
    public class AccountsViewModel : BaseViewModel
    {
        private readonly IAccountsService _accountsService;

        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            protected set { this.RaiseAndSetIfChanged(ref _isLoggingIn, value); }
        }

        public IAccount ActiveAccount
        {
            get { return _accountsService.ActiveAccount; }
            set
            {
                _accountsService.ActiveAccount = value;
                this.RaisePropertyChanged();
            }
        }

        public ReactiveList<IAccount> Accounts { get; private set; }

        public IReactiveCommand<object> LoginCommand { get; private set; }

        public IReactiveCommand<object> GoToAddAccountCommand { get; private set; }

        public IReactiveCommand<object> DeleteAccountCommand { get; private set; }

        public AccountsViewModel(IAccountsService accountsService)
        {
            _accountsService = accountsService;
            Accounts = new ReactiveList<IAccount>(accountsService);
            LoginCommand = ReactiveCommand.Create();
            GoToAddAccountCommand = ReactiveCommand.Create();
            DeleteAccountCommand = ReactiveCommand.Create();

            DeleteAccountCommand.OfType<IAccount>().Subscribe(x =>
            {
                if (Equals(accountsService.ActiveAccount, x))
                    ActiveAccount = null;
                accountsService.Remove(x);
                Accounts.Remove(x);
            });

            LoginCommand.OfType<IAccount>().Subscribe(x =>
            {
                if (Equals(accountsService.ActiveAccount, x))
                    DismissCommand.ExecuteIfCan();
                else
                {
                    ActiveAccount = x;
                    MessageBus.Current.SendMessage(new LogoutMessage());
                    DismissCommand.ExecuteIfCan();
                }
            });

            GoToAddAccountCommand.Subscribe(_ => ShowViewModel(CreateViewModel(typeof(IAddAccountViewModel))));

            this.WhenActivated(d =>
            {
                Accounts.Reset(accountsService);
            });
        }
    }
}
