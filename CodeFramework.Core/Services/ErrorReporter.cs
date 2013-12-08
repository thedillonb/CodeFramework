using System;

namespace CodeFramework.Core.Services
{
	public class ErrorReporter : IErrorReporter
    {
		public void ReportError(string message, Exception e)
		{
			Console.WriteLine("ERROR: " + message + " - " + e.StackTrace);
		}

		public void ReportError(Exception e)
		{
			Console.WriteLine("ERROR: " + e.Message + " - " + e.StackTrace);
		}
    }
}

