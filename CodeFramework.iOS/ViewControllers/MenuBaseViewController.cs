using System.Drawing;
using CodeFramework.Views;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;

namespace CodeFramework.iOS.ViewControllers
{
	public abstract class MenuBaseViewController : ViewModelDrivenDialogViewController
    {
        readonly ProfileButton _profileButton;
        readonly UILabel _title;

        protected MenuBaseViewController()
            : base(false)
        {
            EdgesForExtendedLayout = UIRectEdge.None;
            Style = UITableViewStyle.Plain;
            Autorotate = true;
			_title = new UILabel(new RectangleF(0, 40, 320, 40));
            _title.TextAlignment = UITextAlignment.Left;
            _title.BackgroundColor = UIColor.Clear;
			_title.Font = UIFont.SystemFontOfSize(16f);
            _title.TextColor = UIColor.FromRGB(246, 246, 246);
//            _title.ShadowColor = UIColor.FromRGB(21, 21, 21);
//            _title.ShadowOffset = new SizeF(0, 1);
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
				return _title == null ? base.Title : " " + _title.Text;
            }
            set {
                if (_title != null)
					_title.Text = " " + value;
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

        private void UpdateProfilePicture()
        {
            var size = new SizeF(32, 32);
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft ||
                UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                size = new SizeF(24, 24);
            }

			_profileButton.Frame = new RectangleF(new PointF(0, 4), size);

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(_profileButton);
        }

        protected virtual void ProfileButtonClicked (object sender, System.EventArgs e)
        {
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Add some nice looking colors and effects
            //TableView.SeparatorColor = UIColor.FromRGB(14, 14, 14);
            TableView.TableFooterView = new UIView(new RectangleF(0, 0, View.Bounds.Width, 0));

            var menuBackground = Theme.CurrentTheme.MenuBackground;

            if (menuBackground != null)
            {
                var view = new UIImageView(Theme.CurrentTheme.MenuBackground);
                view.Frame = this.ParentViewController.View.Frame;
                this.ParentViewController.View.InsertSubview(view, 0);
                this.TableView.BackgroundColor = UIColor.Clear;
            }
            else
            {
                TableView.BackgroundColor = UIColor.FromRGB(34, 34, 34);
            }

            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.RowHeight = 54f;

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

        protected override void ScrollViewDidScroll(UIScrollView scrollView)
        {
            var sectionHeaderHeight = 37f;
            if (scrollView.ContentOffset.Y <= sectionHeaderHeight && scrollView.ContentOffset.Y >= 0)
                scrollView.ContentInset = new UIEdgeInsets(-scrollView.ContentOffset.Y, 0, 0, 0);
            else if (scrollView.ContentOffset.Y >= sectionHeaderHeight)
                scrollView.ContentInset = new UIEdgeInsets(-sectionHeaderHeight, 0, 0, 0);
        }

//        protected override void ScrollViewDidScroll(UIScrollView scrollView)
//        {
//            foreach (var cell in TableView.VisibleCells)
//            {
//                var hiddenFrameHeight = scrollView.ContentOffset.Y + 27 - cell.Frame.Y;
//                if (hiddenFrameHeight >= 0 || hiddenFrameHeight <= cell.Frame.Size.Height) {
//                    //[cell maskCellFromTop:hiddenFrameHeight];
//
//                    cell.Layer.Mask = VisibilityMaskWithLocation(hiddenFrameHeight / cell.Frame.Size.Height, cell.Bounds);
//                    cell.Layer.MasksToBounds = true;
//                }
//            }
//        }
//
//        private static CAGradientLayer VisibilityMaskWithLocation(float location, RectangleF bounds)
//        {
//            var mask = new CAGradientLayer();
//            mask.Frame = bounds;
//            mask.Colors = new [] { UIColor.FromWhiteAlpha(1, 0).CGColor, UIColor.FromWhiteAlpha(1, 1).CGColor };
//            mask.Locations = new NSNumber[] { NSNumber.FromFloat(location), NSNumber.FromFloat(location) };
//            return mask;
//        }



        public class MenuSectionView : UIView
        {
            public MenuSectionView(string caption)
                : base(new RectangleF(0, 0, 320, 37))
            {
                //var background = new UIImageView(Theme.CurrentTheme.MenuSectionBackground);
                //background.Frame = this.Frame; 

                this.BackgroundColor = UIColor.Clear;

                var label = new UILabel(); 
                label.BackgroundColor = UIColor.Clear;
                label.Opaque = false; 
                label.TextColor = UIColor.FromRGB(186, 186, 186);
                label.Font =  UIFont.BoldSystemFontOfSize(12f);
                label.Frame = new RectangleF(32, 13, 200, 23); 
                label.Text = " " + caption; 
//            label.ShadowColor = UIColor.FromRGB(0, 0, 0); 
//            label.ShadowOffset = new SizeF(0, -1); 

                var underline = new UIView();
                underline.BackgroundColor = UIColor.FromRGBA(186, 186, 186, 186);
                underline.Frame = new RectangleF(0, 36, 320, 1);

                //this.AddSubview(background); 
                this.AddSubview(underline);
                this.AddSubview(label); 
            }
//
//            public void MaskCellFromTop(float margin)
//            {
//                this.Layer.Mask = VisibilityMaskWithLocation(margin/this.Frame.Size.Height);
//                this.Layer.MasksToBounds = true;
//            }

        

//            - (void)maskCellFromTop:(CGFloat)margin {
//                self.layer.mask = [self visibilityMaskWithLocation:margin/self.frame.size.height];
//                self.layer.masksToBounds = YES;
//            }
//
//            - (CAGradientLayer *)visibilityMaskWithLocation:(CGFloat)location {
//                CAGradientLayer *mask = [CAGradientLayer layer];
//                mask.frame = self.bounds;
//                mask.colors = [NSArray arrayWithObjects:(id)[[UIColor colorWithWhite:1 alpha:0] CGColor], (id)[[UIColor colorWithWhite:1 alpha:1] CGColor], nil];
//                mask.locations = [NSArray arrayWithObjects:[NSNumber numberWithFloat:location], [NSNumber numberWithFloat:location], nil];
//                return mask;
//            }
        }

        protected class MenuElement : StyledStringElement
        {
            public int NotificationNumber { get; set; }

            private static UIColor PrimaryColor = UIColor.FromRGB(233, 233, 233);
//
//            public MenuElement(string title, NSAction tapped, string image)
//                : this(title, tapped, null)
//            {
//                ImageUri = new System.Uri(image);
//            }
//
            public MenuElement(string title, NSAction tapped, UIImage image)
                : base(" " + title, tapped)
            {
                BackgroundColor = UIColor.Clear;
                TextColor = PrimaryColor;
                DetailColor = UIColor.White;
                Image = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                Accessory = UITableViewCellAccessory.None;
            }

            //We want everything to be the same size as far as images go.
            //So, during layout, we'll resize the imageview and pin it to a specific size!
            private class Cell : UITableViewCell
            {
                private const float ImageSize = 16f;
                private UILabel _numberView;

                public int NotificationNumber { get; set; }

                public Cell(UITableViewCellStyle style, string key)
                    : base(style, key)
                {
//                    var v = new UIView(new RectangleF(0, 0, Frame.Width, 1)) { 
//                        BackgroundColor = UIColor.FromRGB(44, 44, 44)
//                    };
//
//                    AddSubview(v);
//                    TextLabel.ShadowColor = UIColor.Black;
//                    TextLabel.ShadowOffset = new SizeF(0, -1); 
                    SelectedBackgroundView = new UIView { BackgroundColor = UIColor.FromRGB(25, 25, 25) };

                    _numberView = new UILabel { BackgroundColor = UIColor.FromRGBA(0.5f, 0.5f, 0.5f, 0.15f) };
                    _numberView.Layer.MasksToBounds = true;
                    _numberView.Layer.CornerRadius = 5f;
                    _numberView.TextAlignment = UITextAlignment.Center;
                    _numberView.TextColor = PrimaryColor;
                    _numberView.Font = UIFont.SystemFontOfSize(12f);
                }

                public override void LayoutSubviews()
                {
                    base.LayoutSubviews();
                    if (ImageView != null)
                    {
                        var center = ImageView.Center;
                        ImageView.Frame = new RectangleF(0, 0, ImageSize, ImageSize);
                        ImageView.Center = new PointF(ImageSize, center.Y);
                        ImageView.TintColor = MenuElement.PrimaryColor;

                        if (TextLabel != null)
                        {
                            var frame = TextLabel.Frame;
                            frame.X = ImageSize * 2;
                            frame.Width += (TextLabel.Frame.X - frame.X);
                            TextLabel.Frame = frame;
                        }
                    }

                    if (NotificationNumber > 0)
                    {
                        _numberView.Frame = new RectangleF(ContentView.Bounds.Width - 44, 15, 34, 22f);
                        _numberView.Text = NotificationNumber.ToString();
                        AddSubview(_numberView);
                    }
                    else
                    {
                        _numberView.RemoveFromSuperview();
                    }
                }
            }

            public override UITableViewCell GetCell(UITableView tv)
            {
                var cell = base.GetCell(tv) as Cell;
                cell.NotificationNumber = NotificationNumber;
                return cell;
            }

            protected override UITableViewCell CreateTableViewCell(UITableViewCellStyle style, string key)
            {
                return new Cell(style, key);
            }
        }
    }
}

