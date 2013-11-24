using System;
using CodeFramework.Core.ViewModels;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;

namespace CodeFramework.Core.ViewModels
{
	public abstract class LoadableViewModel : BaseViewModel, ILoadableViewModel
	{
		private readonly ICommand _loadCommand;
		private bool _isLoading;

		public ICommand LoadCommand
		{
			get { return _loadCommand; }
		}

		public bool IsLoading
		{
			get { return _isLoading; }
			set { _isLoading = value; RaisePropertyChanged(() => IsLoading); }
		}

		protected LoadableViewModel()
		{
			_loadCommand = new MvxCommand<bool?>(async forceCacheInvalidation =>
				{
					try
					{
						IsLoading = true;
						await Load(forceCacheInvalidation ?? false);
					}
					catch (Exception)
					{
						Console.WriteLine("I had trouble!!!");
						throw;
					}
					finally
					{
						IsLoading = false;
					}
				}, _ => IsLoading == false);
		}

		protected abstract Task Load(bool forceCacheInvalidation);
	}
}

