using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeFramework.Data;
using System.Collections.Generic;
using CodeFramework.Elements;

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
        protected class AccountElement : UserElement
        {
            public IAccount Account { get; private set; }

            public AccountElement(IAccount account)
                : base(account.Username, string.Empty, string.Empty, account.AvatarUrl)
            {
                Account = account;
            }
        }
    }
}

