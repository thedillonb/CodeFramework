using System;
using System.Threading.Tasks;

namespace CodeFramework.Core.ViewModels
{
	public abstract class FileSourceViewModel : LoadableViewModel
    {
		private static string[] BinaryMIMEs = new string[] 
		{ 
			"image/", "video/", "audio/", "model/", "application/pdf", "application/zip", "application/gzip"
		};

		private string _filePath;
		private string _contentPath;

		public string FilePath
		{
			get { return _filePath; }
			protected set 
			{
				_filePath = value;
				RaisePropertyChanged(() => FilePath);
			}
		}

		public string ContentPath
		{
			get { return _contentPath; }
			protected set 
			{
				_contentPath = value;
				RaisePropertyChanged(() => ContentPath);
			}
		}

		protected string CreateContentFile()
		{
			var html = System.IO.File.ReadAllText("SourceBrowser/index.html");
			var filled = html.Replace("{CODE_PATH}", "file://" + FilePath);
			var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "source.html");
			System.IO.File.WriteAllText(filepath, filled, System.Text.Encoding.UTF8);
			return filepath;
		}

		protected static bool IsBinary(string mime)
		{
			var lowerMime = mime.ToLower();
			foreach (var m in BinaryMIMEs)
			{
				if (lowerMime.StartsWith(m))
					return true;
			}

			return false;
		}
    }
}

