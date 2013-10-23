using System;

namespace CodeFramework.Services
{
    public interface IErrorReporter
    {
        void ReportError(Exception e);
    }
}

