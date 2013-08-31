using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeFramework.Data;
using System.Collections.Generic;

namespace CodeFramework.Controllers
{
    public abstract class BaseAccountsController : BaseDialogViewController
    {
        protected BaseAccountsController() : base(true)
        {
            Title = "Accounts";
        }

        /// <summary>
        /// Called when the accounts need to be populated
        /// </summary>
        /// <returns>The accounts.</returns>
        protected abstract List<AccountElement> PopulateAccounts();

        /// <summary>
        /// Called when the "Add Account button is clicked"
        /// </summary>
        protected abstract void AddAccountClicked();

        /// <summary>
        /// Called when an account is deleted
        /// </summary>
        /// <param name="account">Account.</param>
        protected abstract void AccountDeleted(IAccount account);

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var root = new RootElement(Title);
            var accountSection = new Section();
            accountSection.AddAll(PopulateAccounts());
            root.Add(accountSection);

            var addAccountSection = new Section();
            addAccountSection.Add(new StyledStringElement("Add Account".t(), AddAccountClicked));
            root.Add(addAccountSection);

            Root = root;
        }


        public override Source CreateSizingSource(bool unevenRows)
        {
            return new EditSource(this);
        }

        private void Delete(Element element)
        {
            var accountElement = element as AccountElement;
            if (accountElement == null)
                return;

            //Remove the designated username
            AccountDeleted(accountElement.Account);
        }

        private class EditSource : Source
        {
            private readonly BaseAccountsController _parent;
            public EditSource(BaseAccountsController dvc) 
                : base (dvc)
            {
                _parent = dvc;
            }

            public override bool CanEditRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                return (indexPath.Section == 0);
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                if (indexPath.Section == 0)
                    return UITableViewCellEditingStyle.Delete;
                return UITableViewCellEditingStyle.None;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                switch (editingStyle)
                {
                    case UITableViewCellEditingStyle.Delete:
                        var section = _parent.Root[indexPath.Section];
                        var element = section[indexPath.Row];
                        _parent.Delete(element);
                        section.Remove(element);
                        break;
                }
            }
        }

        /// <summary>
        /// An element that represents an account object
        /// </summary>
        protected class AccountElement : StyledStringElement
        {
            public IAccount Account { get; private set; }

            public AccountElement(IAccount account)
                : base(account.Username)
            {
                Account = account;
                Image = Images.Misc.Anonymous;
                if (!string.IsNullOrEmpty(Account.AvatarUrl))
                    this.ImageUri = new Uri(Account.AvatarUrl);
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
                    ImageView.Frame = new System.Drawing.RectangleF(5, 5, 32, 32);
                    ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
                    ImageView.Layer.CornerRadius = 4.0f;
                    ImageView.Layer.MasksToBounds = true;
                    TextLabel.Frame = new System.Drawing.RectangleF(42, TextLabel.Frame.Y, TextLabel.Frame.Width, TextLabel.Frame.Height);
                    if (DetailTextLabel != null)
                        DetailTextLabel.Frame = new System.Drawing.RectangleF(42, DetailTextLabel.Frame.Y, DetailTextLabel.Frame.Width, DetailTextLabel.Frame.Height);
                }
            }
        }
    }
}

