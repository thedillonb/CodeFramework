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
		private static readonly string Body = @"<html><head><style>* { white-space: pre-wrap; font-family: Helvetica; font-size: 12px;} body { margin: 10px; } #shit { width: 100%; } " +
		                                      @"img { max-width: 100%; } p { margin: 0; padding: 0; } pre, li, ul { white-space: pre-wrap; word-wrap: break-word; } ul { padding-left: 1em; }</style>" +
		                                      @"<meta name=""viewport"" content=""width=device-width, initial-scale=1, maximum-scale=1""><script>" +
		                                      @"function size() { return document.getElementById('shit').scrollHeight + 20; } " +
		                                      @"function rs() { document.location.href = 'app://resize'; }; window.onsize = rs;" +
		                                      @"var h = 0; setInterval(function() { if (size() != h) { h = size(); rs(); } }, 100);</script></head><body><div id='shit'></div></body></html>";
		private bool _isLoaded = false;
		private string _value;	

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
				_height = float.Parse(webView.EvaluateJavascript("size();"));
				if (GetImmediateRootElement() != null)
					GetImmediateRootElement().Reload(this, UITableViewRowAnimation.None);
				return false;
			}

			if (!request.Url.AbsoluteString.StartsWith("file://"))
				return false;

			return true;
		}

		private void LoadContent(string content)
		{
			content = System.Web.HttpUtility.JavaScriptStringEncode(content);
			webView.EvaluateJavascript("document.getElementById('shit').innerHTML = '" + content + "';");
		}

		public WebElement () : base (string.Empty)
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

			webView.LoadHtmlString(Body, new NSUrl(""));

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
}

