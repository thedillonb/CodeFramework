using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace CodeFramework.iOS.Elements
{
    public class WebElement : Element, IElementSizing
    {
        private readonly UIWebView webView = null;
        private float _height;
        readonly NSString key;
        private bool _isLoaded = false;
        private string _value;

        public Action<string> UrlRequested;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_isLoaded)
                    LoadContent(value);
            }
        }

        private bool ShouldStartLoad (NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            if (request.Url.AbsoluteString.StartsWith("app://resize"))
            {
                var size = webView.EvaluateJavascript("size();");
                if (size != null)
                    float.TryParse(size, out _height);

                if (GetImmediateRootElement() != null)
                    GetImmediateRootElement().Reload(this, UITableViewRowAnimation.None);
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

        private void LoadContent(string content)
        {
            content = System.Web.HttpUtility.JavaScriptStringEncode(content);
            webView.EvaluateJavascript("ins('" + content + "');");
        }

        public WebElement (string content) : base (string.Empty)
        {
            key = new NSString("rbhtml_row");
            webView = new UIWebView();
            webView.ScrollView.ScrollEnabled = false;
            webView.ScrollView.Bounces = false;
            webView.ShouldStartLoad = (w, r, n) => ShouldStartLoad(r, n);
            webView.LoadFinished += (sender, e) => {
                if (!string.IsNullOrEmpty(_value))
                    LoadContent(_value);
                _isLoaded = true;
            };

            webView.LoadHtmlString(content, new NSUrl(""));

        }

        protected override NSString CellKey {
            get {
                return key;
            }
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell (CellKey);
            if (cell == null){
                cell = new UITableViewCell (UITableViewCellStyle.Default, CellKey);
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                webView.AutoresizingMask = UIViewAutoresizing.All;
            }  

            webView.Frame = new RectangleF(0, 0, cell.ContentView.Frame.Width, cell.ContentView.Frame.Height);
            webView.RemoveFromSuperview();
            cell.ContentView.AddSubview (webView);
            cell.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            return cell;
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath){
            return _height;
        }

    }

    public class WebElement2 : Element, IElementSizing
    {
        private readonly UIWebView webView = null;
        private float _height;
        readonly NSString key;
        private bool _isLoaded = false;
        private string _value;

        public Action<string> UrlRequested;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_isLoaded)
                    LoadContent(value);
            }
        }

        private bool ShouldStartLoad (NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            if (request.Url.AbsoluteString.StartsWith("app://resize"))
            {
                var size = webView.EvaluateJavascript("size();");
                if (size != null)
                    float.TryParse(size, out _height);

                if (GetImmediateRootElement() != null)
                    GetImmediateRootElement().Reload(this, UITableViewRowAnimation.None);
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

        private void LoadContent(string content)
        {
            webView.EvaluateJavascript("var a = " + content + "; ins(a);");
        }

        public WebElement2 (string content) : base (string.Empty)
        {
            key = new NSString("webelement2");
            webView = new UIWebView();
            webView.ScrollView.ScrollEnabled = false;
            webView.ScrollView.Bounces = false;
            webView.ShouldStartLoad = (w, r, n) => ShouldStartLoad(r, n);
            webView.LoadFinished += (sender, e) => {
                if (!string.IsNullOrEmpty(_value))
                    LoadContent(_value);
                _isLoaded = true;
            };

            webView.LoadHtmlString(content, new NSUrl(""));

        }

        protected override NSString CellKey {
            get {
                return key;
            }
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell (CellKey);
            if (cell == null){
                cell = new UITableViewCell (UITableViewCellStyle.Default, CellKey);
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                webView.AutoresizingMask = UIViewAutoresizing.All;
            }  

            webView.Frame = new RectangleF(0, 0, cell.ContentView.Frame.Width, cell.ContentView.Frame.Height);
            webView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            webView.RemoveFromSuperview();
            cell.ContentView.AddSubview (webView);
            cell.ContentView.AutosizesSubviews = true;
            cell.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            return cell;
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath){
            return _height;
        }

    }
}

