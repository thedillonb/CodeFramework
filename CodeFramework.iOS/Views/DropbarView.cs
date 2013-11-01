using CodeFramework.iOS;
using MonoTouch.UIKit;
using System.Drawing;

namespace CodeFramework.Views
{
    public class DropbarView : UIView
    {
        private readonly UIView _img;
        private readonly float _height;

        public DropbarView(float width)
            : base (new RectangleF(0, 0, width, 0f))
        {
            this.ClipsToBounds = false;
            var background = Theme.CurrentTheme.DropbarBackground;
            _height = background.Size.Height;

            _img = new UIView();
            _img.BackgroundColor = UIColor.FromPatternImage(background);
            _img.Layer.MasksToBounds = false;
            _img.Layer.ShadowColor = UIColor.Black.CGColor;
            _img.Layer.ShadowOpacity = 0.3f;
            _img.Layer.ShadowOffset = new SizeF(0, 2f);
            AddSubview(_img);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _img.Frame = new RectangleF(0, 0, Bounds.Width, _height);
        }

    }
}

