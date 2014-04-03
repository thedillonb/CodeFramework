using MonoTouch.UIKit;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace CodeFramework.iOS.Views
{
    public class ScrollingToolbarView : UIView
    {
        private const float PADDING = 10f;
        private readonly UIScrollView _scrollView;
        private readonly IEnumerable<UIButton> _buttons;

        public ScrollingToolbarView(RectangleF rect, IEnumerable<UIButton> buttons)
            : base(rect)
        {
            _buttons = buttons;
            this.AutosizesSubviews = true;
            _scrollView = new UIScrollView(new RectangleF(0, 0, this.Frame.Width, this.Frame.Height));
            _scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            _scrollView.UserInteractionEnabled = true;
            _scrollView.ExclusiveTouch = true;
            _scrollView.CanCancelContentTouches = true;
            _scrollView.DelaysContentTouches = true;
            _scrollView.ShowsHorizontalScrollIndicator = false;
            _scrollView.ShowsVerticalScrollIndicator = false;

            var line = new UIView(new RectangleF(0, 0, rect.Width, 0.5f));
            line.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleBottomMargin;
            line.BackgroundColor = UIColor.LightGray;
            Add(line);

            foreach (var button in buttons)
                _scrollView.Add(button);

            Add(_scrollView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            foreach (var button in _buttons)
                button.SizeToFit();

            float left = 0;
            foreach (var button in _buttons)
            {
                var frame = button.Frame;
                frame.X = PADDING + left;
                frame.Y = (this.Bounds.Height - button.Frame.Height) / 2;

                var title = button.Title(UIControlState.Normal);
                if (title != null && title.Length > 3)
                    frame.Width = frame.Width + 10f;
                button.Frame = frame;
                left = button.Frame.Right;
            }

            _scrollView.ContentSize = new SizeF(left + PADDING, this.Frame.Height);
        }
    }
}

