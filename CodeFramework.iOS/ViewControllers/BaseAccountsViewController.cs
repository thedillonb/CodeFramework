using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeFramework.Data;
using System.Collections.Generic;
using CodeFramework.Elements;

namespace CodeFramework.ViewControllers
{
    public abstract class BaseAccountsViewController : BaseDialogViewController
    {
        protected BaseAccountsViewController() : base(true)
        {
            Title = "Accounts";
        }

        /// <summary>
        /// Called when the accounts need to be populated
        /// </summary>
        /// <returns>The accounts.</returns>
        protected List<AccountElement> PopulateAccounts()
        {
            var accounts = new List<AccountElement>();   
            foreach (var account in Accounts.Instance)
            {
                var thisAccount = account;
                var t = new AccountElement(thisAccount);
                t.Tapped += () => AccountSelected(thisAccount);

                //Check to see if this account is the active account. Application.Account could be null 
                //so make it the target of the equals, not the source.
                if (thisAccount.Equals(Accounts.Instance.ActiveAccount))
                    t.Accessory = UITableViewCellAccessory.Checkmark;
                accounts.Add(t);
            }
            return accounts;
        }

        /// <summary>
        /// Called when an account is selected
        /// </summary>
        /// <param name="account">Account.</param>
        protected abstract void AccountSelected(Account account);

        /// <summary>
        /// Called when the "Add Account button is clicked"
        /// </summary>
        protected abstract void AddAccountClicked();

        /// <summary>
        /// Called when an account is deleted
        /// </summary>
        /// <param name="account">Account.</param>
        protected void AccountDeleted(Account account)
        {
            //Remove the designated username
            var thisAccount = account;
            Accounts.Instance.Remove(thisAccount);

            if (Accounts.Instance.ActiveAccount != null && Accounts.Instance.ActiveAccount.Equals(thisAccount))
            {
                NavigationItem.LeftBarButtonItem.Enabled = false;
                Accounts.Instance.SetActiveAccount(null);
            }
        }

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
            private readonly BaseAccountsViewController _parent;
            public EditSource(BaseAccountsViewController dvc) 
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
            public Account Account { get; private set; }

            public AccountElement(Account account)
                : base(account.Username, string.Empty, string.Empty, account.AvatarUrl)
            {
                Account = account;
            }
        }
    }
}

