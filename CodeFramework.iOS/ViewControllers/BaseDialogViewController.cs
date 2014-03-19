using CodeFramework.iOS;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

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
			EdgesForExtendedLayout = UIRectEdge.None;
            Autorotate = true;
			SearchPlaceholder = "Search";
			//AutoHideSearch = true;
            Style = UITableViewStyle.Grouped;
            NavigationItem.BackBarButtonItem = new UIBarButtonItem();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (Title != null && Root != null)
                Root.Caption = Title;
        }

		public override DialogViewController.Source CreateSizingSource(bool unevenRows)
		{
			if (unevenRows)
				return (DialogViewController.Source)new SizingSource(this);
			else
				return (DialogViewController.Source)new Source(this);
		}

		protected new class SizingSource : DialogViewController.SizingSource
		{
            private readonly BaseDialogViewController _parent;
            public SizingSource (BaseDialogViewController controller) : base (controller)
            {
                _parent = controller;
            }

			public override float GetHeightForFooter (UITableView tableView, int sectionIdx)
			{
				var section = Root[sectionIdx];
				if (Container.Style == UITableViewStyle.Grouped && section.FooterView == null && string.IsNullOrEmpty(section.Footer))
					return 3;
				return base.GetHeightForFooter(tableView, sectionIdx);
			}

            public override void Scrolled(UIScrollView scrollView)
            {
                base.Scrolled(scrollView);
                _parent.ScrollViewDidScroll(scrollView);
            }

		}

        protected virtual void ScrollViewDidScroll(UIScrollView scrollView)
        {
        }

		protected new class Source : DialogViewController.Source
		{
            private readonly BaseDialogViewController _parent;
            public Source (BaseDialogViewController controller) : base (controller) 
            {
                _parent = controller;
            }

			public override float GetHeightForFooter (UITableView tableView, int sectionIdx)
			{
				var section = Root[sectionIdx];
				if (Container.Style == UITableViewStyle.Grouped && section.FooterView == null && string.IsNullOrEmpty(section.Footer))
					return 3;
				return base.GetHeightForFooter(tableView, sectionIdx);
			}

            public override void Scrolled(UIScrollView scrollView)
            {
                base.Scrolled(scrollView);
                _parent.ScrollViewDidScroll(scrollView);
            }
		}
    }
}

