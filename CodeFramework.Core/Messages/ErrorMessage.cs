using System;

namespace CodeFramework.Core.Messages
{
    public class ErrorMessage
    {
        public Exception Error { get; private set; }

        public ErrorMessage(Exception error) 
        {
            Error = error;
        }
    }
}

