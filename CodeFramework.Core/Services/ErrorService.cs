using System;
using Cirrious.CrossCore;
using System.Diagnostics;

namespace CodeFramework.Core.Services
{
	public class ErrorService : IErrorService
    {
		public void ReportError(Exception e)
		{
			if (Debugger.IsAttached)
			{
				Debugger.Break();
			}

			Mvx.Resolve<IAnalyticsService>().LogException(e);
		}
    }
}

