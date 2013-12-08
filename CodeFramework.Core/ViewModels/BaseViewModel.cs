using System;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using CodeFramework.Core.Messages;

namespace CodeFramework.Core.ViewModels
{
    /// <summary>
    ///    Defines the BaseViewModel type.
    /// </summary>
    public abstract class BaseViewModel : MvxViewModel
    {
		/// <summary>
		/// Gets the ViewModelTxService
		/// </summary>
		/// <value>The tx sevice.</value>
		protected IViewModelTxService TxSevice
		{
			get { return GetService<IViewModelTxService>(); }
		}

		/// <summary>
		/// Gets the messenger service
		/// </summary>
		/// <value>The messenger.</value>
		protected IMvxMessenger Messenger
		{
			get { return GetService<IMvxMessenger>(); }
		}

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>An instance of the service.</returns>
		protected TService GetService<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }

        protected void ReportError(Exception e)
        {
			Messenger.Publish(new ErrorMessage(this) { Error = e });
			GetService<IErrorReporter>().ReportError(e);
        }

        protected void ReportError(string message, Exception e)
        {
			var newException = new Exception(message, e);
			Messenger.Publish(new ErrorMessage(this) { Error = newException });
			GetService<IErrorReporter>().ReportError(newException);

        }
    }
}
