using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CodeFramework.iOS
{
    public partial class NewsCellView : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("NewsCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("NewsCellView");
		public static readonly UIEdgeInsets EdgeInsets = new UIEdgeInsets(0, 48f, 0, 0);

        public NewsCellView(IntPtr handle) : base(handle)
        {
        }

		public void Set(string name, UIImage img, string time, UIImage actionImage, NSMutableAttributedString header, NSMutableAttributedString body, OHAttributedLabelDelegate headerDelegate, OHAttributedLabelDelegate bodyDelegate)
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

			this.Header.Delegate = headerDelegate;
			this.Body.Delegate = bodyDelegate;
		}

		public void SetHeaderAlignment(bool center)
		{
			this.Header.CenterVertically = center;
		}

        public static NewsCellView Create()
        {
			var cell = (NewsCellView)Nib.Instantiate(null, null)[0];
			cell.Body.SetUnderlineLinks(false);
			cell.Header.SetUnderlineLinks(false);
			cell.SeparatorInset = EdgeInsets;
			//cell.Header.CenterVertically = true;
			return cell;
        }
    }
}

