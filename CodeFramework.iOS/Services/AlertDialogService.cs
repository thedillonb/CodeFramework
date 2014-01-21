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
            alert.CancelButtonIndex = alert.AddButton("No");
            var ok = alert.AddButton("Yes");
            alert.Clicked += (sender, e) => action(e.ButtonIndex == ok);
            alert.Show();
        }

        public void Alert(string title, string message, Action dismissed = null)
        {
            MonoTouch.Utilities.ShowAlert(title, message, dismissed);
        }

        public void PromptTextBox(string title, string message, string defaultValue, string okTitle, Action<string> action)
        {
            var alert = new UIAlertView();
            alert.Title = title;
            alert.Message = message;
            alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
            var cancelButton = alert.AddButton("Cancel".t());
            var okButton = alert.AddButton(okTitle);
            alert.CancelButtonIndex = cancelButton;
            alert.DismissWithClickedButtonIndex(cancelButton, true);
            alert.GetTextField(0).Text = defaultValue;
            alert.Clicked += (s, e) =>
            {
                if (e.ButtonIndex == okButton)
                    action(alert.GetTextField(0).Text);
            };
            alert.Show();
        }
    }
}

