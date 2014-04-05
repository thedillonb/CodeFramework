using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace CodeFramework.iOS
{
    public partial class NewsCellView : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("NewsCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("NewsCellView");
        public static readonly UIEdgeInsets EdgeInsets = new UIEdgeInsets(0, 48f, 0, 0);

        public static UIFont TimeFont
        {
            get { return UIFont.SystemFontOfSize(12f * Theme.CurrentTheme.FontSizeRatio); }
        }

        public static UIFont HeaderFont
        {
            get { return UIFont.SystemFontOfSize(13f * Theme.CurrentTheme.FontSizeRatio); }
        }

        public static UIFont BodyFont
        {
            get { return UIFont.SystemFontOfSize(13f * Theme.CurrentTheme.FontSizeRatio); }
        }

        public class Link
        {
            public NSRange Range;
            public NSAction Callback;
            public int Id;
        }

        public NewsCellView(IntPtr handle) : base(handle)
        {
        }

        public void Set(string name, UIImage img, string time, UIImage actionImage, 
            NSMutableAttributedString header, NSMutableAttributedString body, 
            OHAttributedLabelDelegate headerDelegate, OHAttributedLabelDelegate bodyDelegate,
            IEnumerable<Link> headerLinks, IEnumerable<Link> bodyLinks)
        {
            this.Image.Image = img;
            this.Time.Text = time;
            this.ActionImage.Image = actionImage;

            if (header == null)
                header = new NSMutableAttributedString();
            if (body == null)
                body = new NSMutableAttributedString();

            this.Header.AttributedText = header;
            this.Body.AttributedText = body;
            this.Body.Hidden = body.Length == 0;

            this.Header.RemoveAllCustomLinks();
            this.Body.RemoveAllCustomLinks();
            this.Header.Delegate = headerDelegate;
            this.Body.Delegate = bodyDelegate;

            foreach (var b in headerLinks)
                this.Header.AddCustomLink(new NSUrl(b.Id.ToString()), b.Range);

            foreach (var b in bodyLinks)
                this.Body.AddCustomLink(new NSUrl(b.Id.ToString()), b.Range);
        }

        public void SetHeaderAlignment(bool center)
        {
            this.Header.CenterVertically = center;
        }

        public static NewsCellView Create()
        {
            var cell = (NewsCellView)Nib.Instantiate(null, null)[0];
            cell.Body.SetUnderlineLinks(false);
            cell.Body.LinkColor = CodeFramework.iOS.Elements.NewsFeedElement.LinkColor;
            cell.Header.SetUnderlineLinks(false);
            cell.Header.LinkColor = CodeFramework.iOS.Elements.NewsFeedElement.LinkColor;
            cell.SeparatorInset = EdgeInsets;

            // Special for large fonts
            if (Theme.CurrentTheme.FontSizeRatio > 1.0f)
            {
                cell.Header.Font = HeaderFont;
                cell.Body.Font = BodyFont;
                cell.Time.Font = TimeFont;

                var timeSectionheight = (float)Math.Ceiling(TimeFont.LineHeight);
                var timeFrame = cell.Time.Frame;
                timeFrame.Height = timeSectionheight;
                cell.Time.Frame = timeFrame;

                var imageFrame = cell.ActionImage.Frame;
                imageFrame.Y += (timeFrame.Height - imageFrame.Height) / 2f;
                cell.ActionImage.Frame = imageFrame;

                var headerSectionheight = (float)Math.Ceiling(TimeFont.LineHeight);
                var headerFrame = cell.Header.Frame;
                headerFrame.Height = headerSectionheight * 2f + (float)Math.Ceiling(3f * Theme.CurrentTheme.FontSizeRatio);
                headerFrame.Y = 6 + timeFrame.Height + 5f;
                cell.Header.Frame = headerFrame;

                var picFrame = cell.Image.Frame;
                picFrame.Y = 6 + timeFrame.Height + 5f;
                picFrame.Y += (headerFrame.Height - picFrame.Height) / 2f;
                cell.Image.Frame = picFrame;

                var bodyFrame = cell.Body.Frame;
                bodyFrame.Y = headerFrame.Y + headerFrame.Height + 4f;
                cell.Body.Frame = bodyFrame;
            }

            //cell.Header.CenterVertically = true;
            return cell;
        }
    }
}

