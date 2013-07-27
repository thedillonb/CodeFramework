using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace CodeFramework.Elements
{
    public class MultilinedElement : CustomElement
    {
        private const float PaddingY = 12f;
        private const float PaddingX = 8f;

        public string Value { get; set; }
        public UIFont CaptionFont { get; set; }
        public UIFont ValueFont { get; set; }
        public UIColor CaptionColor { get; set; }
        public UIColor ValueColor { get; set; }

        public MultilinedElement(string caption, string value)
            : this(caption)
        {
            Value = value;
        }

        public MultilinedElement(string caption)
            : base(UITableViewCellStyle.Default, "multilinedelement")
        {
            Caption = caption;
            BackgroundColor = UIColor.FromRGB(247, 247, 247);
            CaptionFont = UIFont.BoldSystemFontOfSize(15f);
            ValueFont = UIFont.SystemFontOfSize(14f);
            CaptionColor = ValueColor = UIColor.FromRGB(41, 41, 41);
        }

        public override void Draw(RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, UIView view)
        {
            CaptionColor.SetColor();
            var width = bounds.Width - PaddingX * 2;
            var textHeight = Caption.MonoStringHeight(CaptionFont, width);
            view.DrawString(Caption, new RectangleF(PaddingX, PaddingY, width, bounds.Height - PaddingY * 2), CaptionFont, UILineBreakMode.WordWrap);

            if (Value != null)
            {
                ValueColor.SetColor();
                var valueOrigin = new PointF(PaddingX, PaddingY + textHeight + 6f);
                var valueSize = new SizeF(width, bounds.Height - valueOrigin.Y);
                view.DrawString(Value, new RectangleF(valueOrigin, valueSize), ValueFont, UILineBreakMode.WordWrap);
            }
        }

        public override float Height(System.Drawing.RectangleF bounds)
        {
            var width = bounds.Width - PaddingX * 2;
            if (IsTappedAssigned)
                width -= 20f;

            var textHeight = Caption.MonoStringHeight(CaptionFont, width);

            if (Value != null)
            {
                textHeight += 6f;
                textHeight += Value.MonoStringHeight(ValueFont, width);
            }

            return textHeight + PaddingY * 2;
        }
    }
}

