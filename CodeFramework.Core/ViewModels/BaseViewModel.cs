using System;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using CodeFramework.Core.Services;

namespace CodeFramework.Core.ViewModels
{
    /// <summary>
    ///    Defines the BaseViewModel type.
    /// </summary>
    public abstract class BaseViewModel : MvxViewModel
    {
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>An instance of the service.</returns>
        public TService GetService<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }

        protected static void ReportError(Exception e)
        {
            Console.WriteLine(e.Message);
            //Mvx.Resolve<IErrorReporter>().ReportError(e);
        }

        protected static void ReportError(string message, Exception e)
        {
            Console.WriteLine(message + ": " + e.Message);
            //Mvx.Resolve<IErrorReporter>().ReportError(message, e);
        }
    }
}
