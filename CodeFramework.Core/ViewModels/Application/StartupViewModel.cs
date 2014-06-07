using System;
using System.Linq;
using System.Threading.Tasks;
using CodeFramework.Core.Data;
using CodeFramework.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeFramework.Core.ViewModels.Application
{
    public abstract class StartupViewModel : LoadableViewModel
    {
        public static Func<IAccount, Task> AttemptLogin;

        protected readonly IAccountsService AccountsService;

        public IReactiveCommand GoToAccountsCommand { get; private set; }

        public IReactiveCommand GoToNewUserCommand { get; private set; }

        public IReactiveCommand GoToMainCommand { get; private set; }

        public IReactiveCommand BecomeActiveWindowCommand { get; private set; }

        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            protected set { this.RaiseAndSetIfChanged(ref _isLoggingIn, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            protected set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        private Uri _imageUrl;
        public Uri ImageUrl
        {
            get { return _imageUrl; }
            protected set { this.RaiseAndSetIfChanged(ref _imageUrl, value); }
        }

        protected StartupViewModel(IAccountsService accountsService)
        {
            AccountsService = accountsService;
            GoToMainCommand = new ReactiveCommand();
            GoToAccountsCommand = new ReactiveCommand();
            GoToNewUserCommand = new ReactiveCommand();
            BecomeActiveWindowCommand = new ReactiveCommand();

            GoToAccountsCommand.Subscribe(_ => ShowViewModel(CreateViewModel<AccountsViewModel>()));

            LoadCommand.RegisterAsyncTask(x => Load());
        }

        /// <summary>
        /// Gets the default account. If there is not one assigned it will pick the first in the account list.
        /// If there isn't one, it'll just return null.
        /// </summary>
        /// <returns>The default account.</returns>
        protected IAccount GetDefaultAccount()
        {
            return AccountsService.GetDefault();
        }

        private void GoToAccountsOrNewUser()
        {
            if (AccountsService.Any())
                GoToAccountsCommand.Execute(null);
            else
                GoToNewUserCommand.Execute(null);
        }

        private async Task Load()
        {
            var account = AccountsService.GetDefault();

            // Account no longer exists
            if (account == null)
            {
                GoToAccountsOrNewUser();
            }
            else
            {
                try
                {
                    await AttemptLogin(account);
                    AccountsService.SetActiveAccount(account);
                    GoToMainCommand.Execute(null);
                }
                catch
                {
                    GoToAccountsCommand.ExecuteIfCan();
                }
            }
        }
    }
}
