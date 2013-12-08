using System;

namespace CodeFramework.Core.Services
{
    public interface IErrorReporter
    {
		void ReportError(string message, Exception e);

        void ReportError(Exception e);
    }
}

