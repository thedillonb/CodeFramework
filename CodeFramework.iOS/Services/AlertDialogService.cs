using System;
using CodeFramework.Core.Services;
using MonoTouch.UIKit;

namespace CodeFramework.iOS.Services
{
    public class AlertDialogService : IAlertDialogService
    {
        public void PromptYesNo(string title, string message, Action<bool> action)
        {
            var alert = new UIAlertView {Title = title, Message = message};
            alert.CancelButtonIndex = alert.AddButton("Cancel");
            var ok = alert.AddButton("Delete");
            alert.Clicked += (sender, e) => action(e.ButtonIndex == ok);
            alert.Show();
        }

        public void Alert(string title, string message, Action dismissed = null)
        {
            MonoTouch.Utilities.ShowAlert(title, message, dismissed);
        }
    }
}

