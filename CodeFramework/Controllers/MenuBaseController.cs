using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using System.Linq;
using CodeFramework.Elements;
using CodeFramework.Views;

namespace CodeFramework.Controllers
{
    public abstract class MenuBaseController : DialogViewController
    {
        readonly ProfileButton _profileButton;
        readonly UILabel _title;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFramework.Controllers.MenuBaseController"/> class.
        /// </summary>
		public MenuBaseController()
            : base(UITableViewStyle.Plain, new RootElement(string.Empty))
        {
            Autorotate = true;
            _title = new UILabel(new RectangleF(0, 40, 320, 40));
            _title.TextAlignment = UITextAlignment.Left;
            _title.BackgroundColor = UIColor.Clear;
            _title.Font = UIFont.BoldSystemFontOfSize(20f);
            _title.TextColor = UIColor.FromRGB(246, 246, 246);
            _title.ShadowColor = UIColor.FromRGB(21, 21, 21);
            _title.ShadowOffset = new SizeF(0, 1);
            NavigationItem.TitleView = _title;

            _profileButton = new ProfileButton();
            _profileButton.TouchUpInside += ProfileButtonClicked;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title {
            get {
                return _title.Text;
            }
            set {
                _title.Text = value;
                base.Title = value;
            }
        }

        public ProfileButton ProfileButton
        {
            get { return _profileButton; }
        }

		/// <summary>
		/// Invoked when it comes time to set the root so the child classes can create their own menus
		/// </summary>
		protected abstract void CreateMenuRoot();

        /// <summary>
        /// A silly helper to avoid writing out the pushview line
        /// </summary>
        /// <param name="controller">Controller.</param>
        protected void NavPush(UIViewController controller)
        {
            NavigationController.PushViewController(controller, false);
        }

        private void UpdateProfilePicture()
        {
            var size = new SizeF(32, 32);
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft ||
                UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                size = new SizeF(24, 24);
            }

            _profileButton.Frame = new RectangleF(new PointF(4, 4), size);

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(_profileButton);
        }

        protected virtual void ProfileButtonClicked (object sender, System.EventArgs e)
        {
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Add some nice looking colors and effects
            TableView.SeparatorColor = UIColor.FromRGB(14, 14, 14);
            TableView.TableFooterView = new UIView(new RectangleF(0, 0, View.Bounds.Width, 0));
            TableView.BackgroundColor = UIColor.FromRGB(34, 34, 34);

            //Prevent the scroll to top on this view
            this.TableView.ScrollsToTop = false;
        }
        
        public override void ViewWillAppear(bool animated)
        {
            UpdateProfilePicture();
            CreateMenuRoot();
            base.ViewWillAppear(animated);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            UpdateProfilePicture();
        }

        protected class MenuElement : StyledElement
        {
            public MenuElement(string title, NSAction tapped, UIImage image)
                : base(title, tapped)
            {
                BackgroundColor = UIColor.Clear;
                TextColor = UIColor.FromRGB(213, 213, 213);
                DetailColor = UIColor.White;
                Image = image;
            }

            public override UITableViewCell GetCell(UITableView tv)
            {
                var cell = base.GetCell(tv);
                cell.TextLabel.ShadowColor = UIColor.Black;
                cell.TextLabel.ShadowOffset = new SizeF(0, -1); 
                cell.SelectedBackgroundView = new UIView { BackgroundColor = UIColor.FromRGB(25, 25, 25) };

                var f = cell.Subviews.Count(x => x.Tag == 1111);
                if (f == 0)
                {
                    var v = new UIView(new RectangleF(0, 0, cell.Frame.Width, 1))
                    { BackgroundColor = UIColor.FromRGB(44, 44, 44), Tag = 1111};
                    cell.AddSubview(v);
                }

                return cell;
            }
        }
    }
}

