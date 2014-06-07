using System;
using System.Reactive.Linq;
using CodeFramework.Core.Data;
using CodeFramework.Core.Messages;
using CodeFramework.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeFramework.Core.ViewModels.Application
{
    public class AccountsViewModel : LoadableViewModel
    {
        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            protected set { this.RaiseAndSetIfChanged(ref _isLoggingIn, value); }
        }

        public ReactiveList<IAccount> Accounts { get; private set; }

        public IReactiveCommand LoginCommand { get; private set; }

        public IReactiveCommand GoToAddAccountCommand { get; private set; }

        public IReactiveCommand DeleteAccountCommand { get; private set; }

        protected AccountsViewModel(IAccountsService accountsService)
        {
            Accounts = new ReactiveList<IAccount>(accountsService);
            LoginCommand = new ReactiveCommand();
            GoToAddAccountCommand = new ReactiveCommand();
            DeleteAccountCommand = new ReactiveCommand();

            DeleteAccountCommand.OfType<IAccount>().Subscribe(x =>
            {
                if (Equals(accountsService.ActiveAccount, x))
                    accountsService.SetActiveAccount(null);
                accountsService.Remove(x);
                Accounts.Remove(x);
            });

            LoginCommand.OfType<IAccount>().Subscribe(x =>
            {
                if (Equals(accountsService.ActiveAccount, x))
                    DismissCommand.ExecuteIfCan();
                else
                {
                    accountsService.SetActiveAccount(x);
                    MessageBus.Current.SendMessage(new LogoutMessage());
                    DismissCommand.ExecuteIfCan();
                }
            });

//            GoToAddAccountCommand.Subscribe(_ =>
//            {
//                var vm = CreateViewModel<LoginViewModel>();
//                vm.WhenAnyValue(x => x.LoggedInAcconut).Skip(1).Subscribe(x => LoadCommand.ExecuteIfCan());
//                ShowViewModel(vm);
//            });

            LoadCommand.Subscribe(x => Accounts.Reset(accountsService));
        }
    }
}
