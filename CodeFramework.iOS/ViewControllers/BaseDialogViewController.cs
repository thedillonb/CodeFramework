using CodeFramework.iOS;
using CodeFramework.iOS.Views;
using CodeFramework.Views;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.CoreGraphics;

namespace CodeFramework.ViewControllers
{
    public class BaseDialogViewController : DialogViewController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDialogViewController"/> class.
        /// </summary>
        /// <param name="push">If set to <c>true</c> push.</param>
        public BaseDialogViewController(bool push)
            : base(new RootElement(""), push)
        {
            Autorotate = true;
			SearchPlaceholder = "Search";
			//AutoHideSearch = true;
            Style = UITableViewStyle.Grouped;
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.BackButton, () => NavigationController.PopViewControllerAnimated(true)));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (Title != null && Root != null)
                Root.Caption = Title;
        }
    }
}

