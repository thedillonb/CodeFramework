using System;
using System.Linq;
using CodeFramework.Core.ViewModels.Application;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using ReactiveUI;

namespace CodeFramework.iOS.Views.Application
{
    public class DefaultStartupView : ViewModelCollectionView<DefaultStartupViewModel>
	{
		public DefaultStartupView()
		{
            Title = "Default Startup View";
			EnableSearch = false;
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            Bind(ViewModel.WhenAnyValue(x => x.StartupViews), x =>
            {
                var e = new StyledStringElement(x);
                e.Tapped += () => ViewModel.SelectedStartupView = x;
                if (string.Equals(ViewModel.SelectedStartupView, x))
                    e.Accessory = UITableViewCellAccessory.Checkmark;
                return e;
            });

		    ViewModel.WhenAnyValue(x => x.SelectedStartupView).Subscribe(x =>
		    {
                if (Root.Count == 0)
                    return;
                foreach (var m in Root[0].Elements.Cast<StyledStringElement>())
                    m.Accessory = (string.Equals(m.Caption, x, StringComparison.OrdinalIgnoreCase)) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                Root.Reload(Root[0], UITableViewRowAnimation.None);
		    });
		}
    }
}

