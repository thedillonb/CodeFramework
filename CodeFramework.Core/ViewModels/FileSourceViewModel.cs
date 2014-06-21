using System.Linq;
using ReactiveUI;
using Xamarin.Utilities.Core.Services;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeFramework.Core.ViewModels
{
	public abstract class FileSourceViewModel : LoadableViewModel
    {
		private static readonly string[] BinaryMIMEs =
		{ 
		    "image/", "video/", "audio/", "model/", "application/pdf", "application/zip", "application/gzip"
		};

		private string _filePath;
		public string FilePath
		{
			get { return _filePath; }
			protected set { this.RaiseAndSetIfChanged(ref _filePath, value); }
		}

        private string _contentPath;
		public string ContentPath
		{
			get { return _contentPath; }
			protected set { this.RaiseAndSetIfChanged(ref _contentPath, value); }
		}

		public string Title { get; set; }

		public string HtmlUrl { get; set; }
//
//		public IReactiveCommand GoToHtmlUrlCommand
//		{
//			get { return new MvxCommand(() => ShowViewModel<WebBrowserViewModel>(new WebBrowserViewModel.NavObject { Url = HtmlUrl }), () => !string.IsNullOrEmpty(HtmlUrl)); }
//		}
//
//        public IReactiveCommand ShareCommand
//		{
//			get
//			{
//				return new MvxCommand(() => GetService<IShareService>().ShareUrl(HtmlUrl), () => !string.IsNullOrEmpty(HtmlUrl));
//			}
//		}

		protected string CreateContentFile()
		{
			var html = System.IO.File.ReadAllText("SourceBrowser/index.html");
			var filled = html.Replace("{CODE_PATH}", "file://" + FilePath + "#" + System.Environment.TickCount);
			var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "source.html");
			System.IO.File.WriteAllText(filepath, filled, System.Text.Encoding.UTF8);
			return filepath;
		}

		protected static string CreatePlainContentFile(string data, string filename)
		{
			var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), filename);
			System.IO.File.WriteAllText(filepath, data, System.Text.Encoding.UTF8);
			return filepath;
		}

		protected static bool IsBinary(string mime)
		{
			var lowerMime = mime.ToLower();
		    return BinaryMIMEs.Any(lowerMime.StartsWith);
		}
    }
}

