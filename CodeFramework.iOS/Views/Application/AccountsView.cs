using System;
using System.Linq;
using System.Reactive.Linq;
using CodeFramework.Core.Data;
using CodeFramework.Core.ViewModels.Application;
using CodeFramework.iOS.Elements;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;

namespace CodeFramework.iOS.Views.Application
{
    public class AccountsView : ViewModelDialogView<AccountsViewModel>
	{
        public AccountsView()
            : base(UITableViewStyle.Plain)
        {
        }

        public override void ViewDidLoad()
        {
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => ViewModel.GoToAddAccountCommand.ExecuteIfCan());
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Images.Cancel, UIBarButtonItemStyle.Plain, (s, e) => ViewModel.DismissCommand.ExecuteIfCan());

            TableView.RowHeight = 74f;
            TableView.SeparatorInset = new UIEdgeInsets(0, TableView.RowHeight, 0, 0);

            base.ViewDidLoad();

            var sec = new Section();
            Root = new RootElement("Accounts") { sec };

            ViewModel.Accounts.ItemsRemoved.Where(x => Root != null && Root.Count > 0).Subscribe(x => 
                sec.Elements.OfType<ProfileElement>().Where(e => Equals(e.Tag, x)).ToList().ForEach(e => sec.Remove(e)));

            ViewModel.Accounts.Changed
                .Where(x => x.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                .Subscribe(_ => sec.Reset(ViewModel.Accounts.Select(x =>
                {
                    var shortenedDomain = new Uri(x.Domain);
                    var element = new ProfileElement(x.Username, shortenedDomain.Host)
                    {
                        Accessory = Equals(ViewModel.ActiveAccount, x) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None,
                        Tag = x,
                        Image = Images.LoginUserUnknown,
                        ImageUri = x.AvatarUrl
                    };
                    element.Tapped += () => ViewModel.LoginCommand.ExecuteIfCan(x);
                    return element;
                })));

            ViewModel.WhenAnyValue(x => x.ActiveAccount, x => x != null)
                .Where(x => NavigationItem != null)
                .Subscribe(x => NavigationItem.LeftBarButtonItem.Enabled = x);
        }

	    public override void ViewWillAppear(bool animated)
	    {
	        base.ViewWillAppear(animated);
	        ViewModel.LoadCommand.ExecuteIfCan();
	    }

	    public override Source CreateSizingSource(bool unevenRows)
        {
            return new DialogDeleteSource(this, x =>
            {
                var profileElement = x as ProfileElement;
                if (profileElement == null)
                    return;

                var account = profileElement.Tag as IAccount;
                if (account != null)
                    ViewModel.DeleteAccountCommand.ExecuteIfCan(account);
            });
        }

        public class DialogDeleteSource : Source
        {
            private readonly Action<Element> _deleteAction;

            public DialogDeleteSource(DialogViewController container, Action<Element> deleteAction)
                : base(container)
            {
                _deleteAction = deleteAction;
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.Delete;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                return true;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                var section = Container.Root[indexPath.Section];
                var element = section[indexPath.Row];
                if (_deleteAction != null)
                    _deleteAction(element);
            }
        }
    }
}

