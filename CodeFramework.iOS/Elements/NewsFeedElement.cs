using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;

namespace CodeFramework.iOS.Elements
{
    public class NewsFeedElement : Element, IElementSizing, IColorizeBackground, IImageUpdated
    {
        private readonly string _name;
        private readonly string _time;
        private readonly Uri _imageUri;
        private UIImage _image;
        private readonly UIImage _actionImage;
        private readonly int _bodyBlocks;
        private readonly Action _tapped;

        private readonly NSMutableAttributedString _attributedHeader;
        private readonly NSMutableAttributedString _attributedBody;
        private readonly List<NewsCellView.Link> _headerLinks;
        private readonly List<NewsCellView.Link> _bodyLinks;

        private readonly LinkDelegate _headerLinkDelegate;
        private readonly LinkDelegate _bodyLinkDelegate;

        public static UIColor LinkColor = Theme.CurrentTheme.MainTitleColor;
        public static UIFont LinkFont = UIFont.BoldSystemFontOfSize(13f);

        private UIImage LittleImage { get; set; }

        public Action<NSUrl> WebLinkClicked;

        public class TextBlock
        {
            public string Value;
            public Action Tapped;
            public UIFont Font;
            public UIColor Color;

            public TextBlock()
            {
            }

            public TextBlock(string value)
            {
                Value = value;
            }

            public TextBlock(string value, Action tapped = null)
                : this (value)
            {
                Tapped = tapped;
            }

            public TextBlock(string value, UIFont font = null, UIColor color = null, Action tapped = null)
                : this(value, tapped)
            {
                Font = font; 
                Color = color;
            }
        }

        public NewsFeedElement(string name, string imageUrl, DateTimeOffset time, IEnumerable<TextBlock> headerBlocks, IEnumerable<TextBlock> bodyBlocks, UIImage littleImage, Action tapped)
            : base(null)
        {
            _name = name;
            _imageUri = new Uri(imageUrl);
            _time = time.ToDaysAgo();
            _actionImage = littleImage;
            _tapped = tapped;

            var header = CreateAttributedStringFromBlocks(headerBlocks);
            _attributedHeader = header.Item1;
            _headerLinks = header.Item2;

            var body = CreateAttributedStringFromBlocks(bodyBlocks);
            _attributedBody = body.Item1;
            _bodyLinks = body.Item2;
            _bodyBlocks = bodyBlocks.Count();

            _headerLinkDelegate = new LinkDelegate(_headerLinks, this);
            _bodyLinkDelegate = new LinkDelegate(_bodyLinks, this);
        }

        private Tuple<NSMutableAttributedString,List<NewsCellView.Link>> CreateAttributedStringFromBlocks(IEnumerable<TextBlock> blocks)
        {
            var attributedString = new NSMutableAttributedString();
            var links = new List<NewsCellView.Link>();

            int lengthCounter = 0;
            int i = 0;

            foreach (var b in blocks)
            {
                UIColor color = null;
                if (b.Color != null)
                    color = b.Color;
                else
                {
                    if (b.Tapped != null)
                        color = LinkColor;
                }

                UIFont font = null;
                if (b.Font != null)
                    font = b.Font.WithSize(b.Font.PointSize * Theme.CurrentTheme.FontSizeRatio);
                else
                {
                    if (b.Tapped != null)
                        font = LinkFont.WithSize(LinkFont.PointSize * Theme.CurrentTheme.FontSizeRatio);
                }

                if (color == null)
                    color = Theme.CurrentTheme.MainTextColor;
                if (font == null)
                    font = NewsCellView.BodyFont;


                var ctFont = new MonoTouch.CoreText.CTFont(font.Name, font.PointSize);
                var str = new NSAttributedString(b.Value, new MonoTouch.CoreText.CTStringAttributes() { ForegroundColor = color.CGColor, Font = ctFont });
                attributedString.Append(str);
                var strLength = str.Length;

                if (b.Tapped != null)
                    links.Add(new NewsCellView.Link { Range = new NSRange(lengthCounter, strLength), Callback = new NSAction(b.Tapped), Id = i++ });

                lengthCounter += strLength;
            }

            return new Tuple<NSMutableAttributedString, List<NewsCellView.Link>>(attributedString, links);
        }

        private class LinkDelegate : OHAttributedLabelDelegate
        {
            private readonly List<NewsCellView.Link> _links;
            private readonly NewsFeedElement _parent;

            public LinkDelegate(List<NewsCellView.Link> links, NewsFeedElement parent)
            {
                _links = links;
                _parent = parent;
            }

            public override bool ShouldFollowLink (OHAttributedLabel sender, NSObject linkInfo)
            {
                var a = (NSUrl)MonoTouch.ObjCRuntime.Runtime.GetNSObject (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend(linkInfo.Handle, MonoTouch.ObjCRuntime.Selector.GetHandle ("URL")));
                try
                {
                    if (a.AbsoluteString.StartsWith("http"))
                    {
                        if (_parent.WebLinkClicked != null)
                            _parent.WebLinkClicked(a);
                    }
                    else
                    {
                        var id = Int32.Parse(a.AbsoluteString);
                        _links[id].Callback();
                    }
                }
                catch (Exception e)
                {
                    MonoTouch.Utilities.LogException("Unable to callback on OHAttributedLabel", e);
                }
                return false;
            }
        }

        private static float CharacterHeight 
        {
            get { return "A".MonoStringHeight(NewsCellView.BodyFont, 1000); }
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            var s = 6f + NewsCellView.TimeFont.LineHeight + 5f + (NewsCellView.HeaderFont.LineHeight * 2) + 4f + 7f;

            if (_attributedBody.Length == 0)
                return s;

            var rec = _attributedBody.GetBoundingRect(new SizeF(tableView.Bounds.Width - 56, 10000), NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.UsesFontLeading, null);
            var height = rec.Height;

            if (_bodyBlocks == 1 && height > (CharacterHeight * 4))
                height = CharacterHeight * 4;

            var descCalc = s + height;
            var ret = ((int)Math.Ceiling(descCalc)) + 1f + 8f;
            return ret;
        }

        private bool IsHeaderMultilined(UITableView tableView)
        {
            var rec = _attributedHeader.GetBoundingRect(new SizeF(tableView.Bounds.Width - 56, 10000), NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.UsesFontLeading, null);
            var height = rec.Height;
            return height > CharacterHeight + 4f; // +4 due to weird buffer space
        }

        protected override NSString CellKey {
            get {
                return new NSString("NewsCellView");
            }
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(CellKey) as NewsCellView ?? NewsCellView.Create();
            return cell;
        }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Selected(dvc, tableView, path);
            if (_tapped != null)
                _tapped();
            tableView.DeselectRow (path, true);
        }

        void IColorizeBackground.WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            var c = cell as NewsCellView;
            if (c == null)
                return;

            if (_image == null && _imageUri != null)
                _image = ImageLoader.DefaultRequestImage(_imageUri, this);

            var isHeaderMultilined = IsHeaderMultilined(tableView);
            c.SetHeaderAlignment(!isHeaderMultilined);
            c.Set(_name, _image, _time, _actionImage, _attributedHeader, _attributedBody, _headerLinkDelegate, _bodyLinkDelegate, _headerLinks, _bodyLinks);
        }

        #region IImageUpdated implementation

        public void UpdatedImage(Uri uri)
        {
            var img = ImageLoader.DefaultRequestImage(uri, this);
            if (img == null)
                return;
            _image = img;

            if (uri == null)
                return;
            var root = GetImmediateRootElement ();
            if (root == null || root.TableView == null)
                return;
            root.TableView.ReloadRows (new [] { IndexPath }, UITableViewRowAnimation.None);
        }

        #endregion
    }
}