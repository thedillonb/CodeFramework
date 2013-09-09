using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace CodeFramework.Elements
{
    public class UserElement : StyledStringElement
    {
        public UserElement(string username, string firstName, string lastName, string avatar)
            : base (username, string.Empty, UITableViewCellStyle.Subtitle)
        {
            var realName = firstName ?? "" + " " + (lastName ?? "");
             if (!string.IsNullOrWhiteSpace(realName))
                Value = realName;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
            Image = Theme.CurrentTheme.AnonymousUserImage;
            if (avatar != null)
                ImageUri = new Uri(avatar);
        }

        
        // We need to create our own cell so we can position the image view appropriately
        protected override UITableViewCell CreateTableViewCell(UITableViewCellStyle style, string key)
        {
            return new PinnedImageTableViewCell(style, key);
        }

        /// <summary>
        /// This class is to make sure the imageview is of a specific size... :(
        /// </summary>
        private class PinnedImageTableViewCell : UITableViewCell
        {
            public PinnedImageTableViewCell(UITableViewCellStyle style, string key) : base(style, key) { }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                ImageView.Frame = new System.Drawing.RectangleF(6, 6, 32, 32);
                ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
                ImageView.Layer.CornerRadius = 4.0f;
                ImageView.Layer.MasksToBounds = true;
                //TextLabel.Frame = new System.Drawing.RectangleF(42, TextLabel.Frame.Y, TextLabel.Frame.Width, TextLabel.Frame.Height);
                //if (DetailTextLabel != null)
                //    DetailTextLabel.Frame = new System.Drawing.RectangleF(42, DetailTextLabel.Frame.Y, DetailTextLabel.Frame.Width, DetailTextLabel.Frame.Height);
            }
        }
    }
}

