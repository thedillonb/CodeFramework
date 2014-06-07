using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using CodeFramework.Core.Data;
using CodeFramework.Core.Services;
using CodeFramework.Core.Utils;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeFramework.Core.ViewModels.Application
{
	public abstract class BaseMenuViewModel : BaseViewModel
	{
	    protected readonly IAccountsService AccountsService;

		public ICommand GoToDefaultTopView
		{
			get
			{
                var startupViewName = AccountsService.ActiveAccount.DefaultStartupView;
				if (!string.IsNullOrEmpty(startupViewName))
				{
					var props = from p in GetType().GetProperties()
	                            let attr = p.GetCustomAttributes(typeof(PotentialStartupViewAttribute), true)
	                            where attr.Length == 1
	                            select new { Property = p, Attribute = attr[0] as PotentialStartupViewAttribute};

					foreach (var p in props)
					{
						if (string.Equals(startupViewName, p.Attribute.Name))
							return p.Property.GetValue(this) as ICommand;
					}
				}

				//Oh no... Look for the last resort DefaultStartupViewAttribute
				var deprop = (from p in GetType().GetProperties()
				              let attr = p.GetCustomAttributes(typeof(DefaultStartupViewAttribute), true)
				              where attr.Length == 1
							  select new { Property = p, Attribute = attr[0] as DefaultStartupViewAttribute }).FirstOrDefault();

				//That shouldn't happen...
				if (deprop == null)
					return null;
				var val = deprop.Property.GetValue(this);
				return val as ICommand;
			}
		}

		public IReactiveCommand DeletePinnedRepositoryCommand { get; private set; }

        public IReactiveList<PinnedRepository> PinnedRepositories { get; private set; }

        protected BaseMenuViewModel(IAccountsService accountsService)
        {
            AccountsService = accountsService;
            DeletePinnedRepositoryCommand = new ReactiveCommand();
            PinnedRepositories = new ReactiveList<PinnedRepository>(AccountsService.ActiveAccount.PinnnedRepositories);

            DeletePinnedRepositoryCommand.OfType<PinnedRepository>()
                .Subscribe(x =>
                {
                    AccountsService.ActiveAccount.PinnnedRepositories.RemovePinnedRepository(x.Id);
                    PinnedRepositories.Remove(x);
                });
        }

        public void Init()
        {
            GoToDefaultTopView.Execute(null);
        }
    }
}
