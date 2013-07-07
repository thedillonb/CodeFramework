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
        protected static string TempDir = System.IO.Path.Combine(MonoTouch.Utilities.BaseDir, "tmp", "source");

        public FileSourceController()
            : base(false)
        {
            Web.DataDetectorTypes = UIDataDetectorType.None;
            Title = "Source";
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //Create the temp directory if it does not exist!
            if (!System.IO.Directory.Exists(TempDir))
                System.IO.Directory.CreateDirectory(TempDir);

            //Do the request
            this.DoWork(Request, ex => ErrorView.Show(this.View, ex.Message));
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            //Remove all files within the temp directory
            if (System.IO.Directory.Exists(TempDir))
                System.IO.Directory.Delete(TempDir, true);
        }

        protected override void OnLoadError(object sender, UIWebErrorArgs e)
        {
            base.OnLoadError(sender, e);

            //Can't load this!
            ErrorView.Show(this.View, "Unable to display this type of file.");
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

        protected void LoadFile(string path)
        {
            var uri = Uri.EscapeUriString("file://" + path) + "#" + Environment.TickCount;
            InvokeOnMainThread(() => Web.LoadRequest(new NSUrlRequest(new NSUrl(uri))));
        }
    }
}

