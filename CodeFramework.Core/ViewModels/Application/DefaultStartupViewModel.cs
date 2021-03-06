using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using CodeFramework.Core.Services;
using CodeFramework.Core.Utils;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeFramework.Core.ViewModels.Application
{
	public class DefaultStartupViewModel : BaseViewModel
    {
		protected readonly IAccountsService AccountsService;

        public IReadOnlyReactiveList<string> StartupViews { get; private set; } 

		private string _selectedStartupView;
		public string SelectedStartupView
		{
			get { return _selectedStartupView; }
			set { this.RaiseAndSetIfChanged(ref _selectedStartupView, value); }
		}

		protected DefaultStartupViewModel(IAccountsService accountsService, Type menuViewModelType)
		{
			AccountsService = accountsService;

            SelectedStartupView = AccountsService.ActiveAccount.DefaultStartupView;
		    this.WhenAnyValue(x => x.SelectedStartupView).Skip(1).Subscribe(x =>
		    {
		        AccountsService.ActiveAccount.DefaultStartupView = x;
		        AccountsService.Update(AccountsService.ActiveAccount);
		        DismissCommand.ExecuteIfCan();
		    });

            StartupViews = new ReactiveList<string>(from p in menuViewModelType.GetRuntimeProperties()
                let attr = p.GetCustomAttributes(typeof(PotentialStartupViewAttribute), true).ToList()
                where attr.Count == 1 && attr[0] is PotentialStartupViewAttribute
                select ((PotentialStartupViewAttribute)attr[0]).Name);
		}
    }
}

