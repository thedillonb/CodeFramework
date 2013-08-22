using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Text;
using CodeFramework.Controllers;
using CodeFramework.Views;

namespace CodeFramework.Controllers
{
    public abstract class FileSourceController : WebViewController
    {
        protected static string TempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "source");

        public FileSourceController()
            : base(false)
        {
            Web.DataDetectorTypes = UIDataDetectorType.None;
            Title = "Source".t();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Create the temp directory if it does not exist!
            if (System.IO.Directory.Exists(TempDir))
                System.IO.Directory.Delete(TempDir, true);
            System.IO.Directory.CreateDirectory(TempDir);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //Do the request
            this.DoWork(Request, ex => ErrorView.Show(this.View, ex.Message));
        }

        protected override void OnLoadError(object sender, UIWebErrorArgs e)
        {
            base.OnLoadError(sender, e);

            //Can't load this!
            ErrorView.Show(this.View, "Unable to display this type of file.".t());
        }

        protected abstract void Request();

        protected void LoadRawData(string data)
        {
            InvokeOnMainThread(delegate {
                var html = System.IO.File.ReadAllText("SourceBrowser/index.html");
                var filled = html.Replace("{DATA}", data);

                var url = NSBundle.MainBundle.BundlePath + "/SourceBrowser";
                url = url.Replace("/", "//").Replace(" ", "%20");

                Web.LoadHtmlString(filled, NSUrl.FromString("file:/" + url + "//"));
            });
        }

        protected void LoadDiffData(string baseText, string newText)
        {
            InvokeOnMainThread(delegate {
                var html = System.IO.File.ReadAllText("Diff/diffindex.html");
                var filled = html.Replace("{BASE_TEXT}", JavaScriptStringEncode(baseText)).Replace("{NEW_TEXT}", JavaScriptStringEncode(newText));
                var url = NSBundle.MainBundle.BundlePath + "/Diff";
                url = url.Replace("/", "//").Replace(" ", "%20");
                Web.LoadHtmlString(filled, NSUrl.FromString("file:/" + url + "//"));
            });
        }

        protected void LoadFile(string path)
        {
            var uri = Uri.EscapeUriString("file://" + path) + "#" + Environment.TickCount;
            InvokeOnMainThread(() => Web.LoadRequest(new NSUrlRequest(new NSUrl(uri))));
        }

        public static string JavaScriptStringEncode (string value)
        {
            return System.Web.HttpUtility.JavaScriptStringEncode(value);
        }
    }
}

