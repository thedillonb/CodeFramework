using System;

namespace CodeFramework.Core.Services
{
    public interface IErrorService
    {
        void ReportError(Exception e);
    }
}

