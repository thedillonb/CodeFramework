using System;
using MonoTouch.UIKit;

namespace CodeFramework.Elements
{
    public class UserElement : SubcaptionElement
    {
        public UserElement(string username, string firstName, string lastName, string avatar)
            : base (username)
        {
            var realName = firstName ?? "" + " " + (lastName ?? "");
             if (!string.IsNullOrWhiteSpace(realName))
                Value = realName;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
            Image = CodeFramework.Images.Misc.Anonymous;
            if (avatar != null)
                ImageUri = new Uri(avatar);
        }
    }
}

