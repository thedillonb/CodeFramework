using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CodeFramework.iOS
{
    public partial class NewsCellView : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("NewsCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("NewsCellView");

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

//
//		private void CreateOrUpdate(RectangleF frame)
//		{
//			if (_label == null)
//			{
//				_label = new OHAttributedLabel(frame);
//				_label.Tag = 100;
//				_label.BackgroundColor = UIColor.Clear;
//				_label.AttributedText = _string;
//				_label.Delegate = _kitty;
//				_label.RemoveAllCustomLinks();
//				_label.SetUnderlineLinks(false);
//				_label.LineBreakMode = UILineBreakMode.WordWrap;
//				if (LinkColor != null)
//					_label.LinkColor = LinkColor;
//
//				foreach (var b in _listToLinks)
//				{
//					_label.AddCustomLink(new NSUrl(b.Id.ToString()), b.Range);
//				}
//
//			}
//			else
//			{
//				_label.Frame = frame;
//			}
//		}

        public static NewsCellView Create()
        {
			var cell = (NewsCellView)Nib.Instantiate(null, null)[0];
			cell.Body.SetUnderlineLinks(false);
			cell.Header.SetUnderlineLinks(false);
			//cell.Header.CenterVertically = true;
			return cell;
        }
    }
}

