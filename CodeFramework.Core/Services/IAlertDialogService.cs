using System;

namespace CodeFramework.Core.Services
{
    public interface IAlertDialogService
    {
        void PromptYesNo(string title, string message, Action<bool> action);

        void Alert(string title, string message, Action dismissed = null);

        void PromptTextBox(string title, string message, string defaultValue, string okTitle, Action<string> action);
    }
}

