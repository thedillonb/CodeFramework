using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace CodeFramework.iOS.ViewComponents
{
    public class SlideUpTitleView : UIView
    {
        private readonly UILabel _label;

        public string Text
        {
            get { return _label.Text; }
            set 
            { 
                _label.Text = value;
                _label.SizeToFit();

                var labelFrame = _label.Frame;
                labelFrame.Height = Frame.Height;
                _label.Frame = labelFrame;

                var f = Frame;
                f.Width = _label.Bounds.Width;
                Frame = f;
            }
        }

        public float Offset
        {
            get { return _label.Frame.Y; }
            set
            {
                if (value < 0)
                    value = 0;
                var f = _label.Frame;
                f.Y = value;
                _label.Frame = f;
            }
        }

        public SlideUpTitleView(float height)
            : base(new RectangleF(0, 0, 10, height))
        {
            AutosizesSubviews = true;
            AutoresizingMask = UIViewAutoresizing.FlexibleHeight;

            //BackgroundColor = UIColor.Red;
            _label = new UILabel(Bounds);
            _label.Font = UIFont.SystemFontOfSize(18f);
            _label.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
            _label.TextAlignment = UITextAlignment.Center;
            _label.LineBreakMode = UILineBreakMode.TailTruncation;
            _label.TextColor = UIColor.White;
            Add(_label);

            Layer.MasksToBounds = true;
        }
    }
}

