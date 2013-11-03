using System.Drawing;
using MonoTouch.UIKit;

namespace CodeFramework.iOS.Views
{
    public class ErrorView : UIView
    {
        private readonly UIImageView _imgView;
        private readonly UILabel _label;

        public static UIFont TitleFont = UIFont.SystemFontOfSize(15f); 

        public string Title 
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        private ErrorView()
        {
            BackgroundColor = UIColor.White;
            _imgView = new UIImageView(Theme.CurrentTheme.WarningImage);
            _label = new UILabel();
            _label.TextAlignment = UITextAlignment.Center;
            _label.LineBreakMode = UILineBreakMode.WordWrap;
            _label.Lines = 0;

            this.Add(_imgView);
            this.Add(_label);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _imgView.Center = new PointF(this.Frame.Width / 2, this.Frame.Height / 2 - _imgView.Frame.Height / 2);
            _label.Frame = new RectangleF(0, 0, this.Frame.Width - 20f, 0);
            _label.SizeToFit();
            _label.Frame = new RectangleF(10, this.Frame.Height / 2, this.Frame.Width - 20f, _label.Frame.Height);
        }

        public static ErrorView Show(UIView parent, string title)
        {
            if (parent == null)
                return null;

            var ror = new ErrorView { Title = title, Frame = parent.Bounds };
            parent.AddSubview(ror);
            ror.SetNeedsDisplay();
            return ror;
        }
    }
}

