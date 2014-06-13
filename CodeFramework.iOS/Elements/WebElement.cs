using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace CodeFramework.iOS.Elements
{
    public class WebElement : Element, IElementSizing
    {
        protected readonly UIWebView WebView = null;
        private float _height;
        protected readonly NSString Key;

        public Action<float> HeightChanged;

        public Action<string> UrlRequested;

        public float Height
        {
            get { return _height; }
        }

        public string Value
        {
            set
            {
                WebView.LoadHtmlString(value, NSBundle.MainBundle.BundleUrl);
            }
        }

        private bool ShouldStartLoad (NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            if (request.Url.AbsoluteString.StartsWith("app://resize"))
            {
                try
                {
                    var size = WebView.EvaluateJavascript("size();");
                    if (size != null)
                        float.TryParse(size, out _height);

                    if (HeightChanged != null)
                        HeightChanged(_height);
                }
                catch
                {
                }

                return false;
            }

            if (!request.Url.AbsoluteString.StartsWith("file://"))
            {
                if (UrlRequested != null)
                    UrlRequested(request.Url.AbsoluteString);
                return false;
            }

            return true;
        }

        public WebElement (string cellKey) 
            : base (string.Empty)
        {
            Key = new NSString(cellKey);
            WebView = new UIWebView();
            WebView.ScrollView.ScrollEnabled = false;
            WebView.ScrollView.Bounces = false;
            WebView.ShouldStartLoad = (w, r, n) => ShouldStartLoad(r, n);

            HeightChanged = (x) => {
                if (GetImmediateRootElement() != null)
                    GetImmediateRootElement().Reload(this, UITableViewRowAnimation.Fade);
            };
        }

        protected override NSString CellKey 
        {
            get { return Key; }
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            return _height;
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell (CellKey);
            if (cell == null){
                cell = new UITableViewCell (UITableViewCellStyle.Default, CellKey);
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                WebView.AutoresizingMask = UIViewAutoresizing.All;
            }  

            WebView.Frame = new RectangleF(0, 0, cell.ContentView.Frame.Width, cell.ContentView.Frame.Height);
            WebView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            WebView.RemoveFromSuperview();
            cell.ContentView.AddSubview (WebView);
            cell.ContentView.Layer.MasksToBounds = true;
            cell.ContentView.AutosizesSubviews = true;
            cell.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            return cell;
        }
    }
}

