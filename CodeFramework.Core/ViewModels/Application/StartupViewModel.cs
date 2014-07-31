using System;
using System.Linq;
using System.Threading.Tasks;
using CodeFramework.Core.Data;
using CodeFramework.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeFramework.Core.ViewModels.Application
{
    public class StartupViewModel : BaseViewModel, ILoadableViewModel
    {
        protected readonly IAccountsService AccountsService;
        protected readonly IAccountValidatorService AccountValidator;

        public IReactiveCommand<object> GoToAccountsCommand { get; private set; }

        public IReactiveCommand<object> GoToNewUserCommand { get; private set; }

        public IReactiveCommand<object> GoToMainCommand { get; private set; }

        public IReactiveCommand BecomeActiveWindowCommand { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

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

        public StartupViewModel(IAccountsService accountsService, IAccountValidatorService accountValidator)
        {
            AccountsService = accountsService;
            AccountValidator = accountValidator;

            GoToMainCommand = ReactiveCommand.Create();
            GoToAccountsCommand = ReactiveCommand.Create();
            GoToNewUserCommand = ReactiveCommand.Create();
            BecomeActiveWindowCommand = ReactiveCommand.Create();

            GoToAccountsCommand.Subscribe(_ => ShowViewModel(CreateViewModel<AccountsViewModel>()));

            GoToNewUserCommand.Subscribe(_ => ShowViewModel(CreateViewModel(typeof(IAddAccountViewModel))));

            GoToMainCommand.Subscribe(_ => ShowViewModel(CreateViewModel(typeof(IMainViewModel))));

            LoadCommand = ReactiveCommand.CreateAsyncTask(x => Load());
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
                    Status = string.Format("Logging in {0}", account.Username);

                    Uri avatarUri;
                    if (Uri.TryCreate(account.AvatarUrl, UriKind.Absolute, out avatarUri))
                        ImageUrl = avatarUri;

                    IsLoggingIn = true;
                    await AccountValidator.Validate(account);
                    AccountsService.ActiveAccount = account;
                    GoToMainCommand.Execute(null);
                }
                catch
                {
                    GoToAccountsCommand.ExecuteIfCan();
                }
                finally
                {
                    IsLoggingIn = false;
                }
            }
        }
    }
}
