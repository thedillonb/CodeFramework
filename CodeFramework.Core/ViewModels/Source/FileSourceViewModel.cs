using System.Linq;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using System.Collections.Generic;

namespace CodeFramework.Core.ViewModels.Source
{
	public abstract class FileSourceViewModel : LoadableViewModel
    {
		private static readonly string[] BinaryMIMEs =
		{ 
		    "image/", "video/", "audio/", "model/", "application/pdf", "application/zip", "application/gzip"
		};

        private SourceItemViewModel _source;
        public SourceItemViewModel SourceItem
		{
            get { return _source; }
            protected set { this.RaiseAndSetIfChanged(ref _source, value); }
		}

        private string _theme;
        public string Theme
        {
            get { return _theme; }
            set { this.RaiseAndSetIfChanged(ref _theme, value); }
        }
            
		public string Title { get; set; }

		public string HtmlUrl { get; set; }

		protected static bool IsBinary(string mime)
		{
			var lowerMime = mime.ToLower();
		    return BinaryMIMEs.Any(lowerMime.StartsWith);
		}

        protected static string CreatePlainContentFile(string data, string filename)
        {
            var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), filename);
            System.IO.File.WriteAllText(filepath, data, System.Text.Encoding.UTF8);
            return filepath;
        }

        public class SourceItemViewModel
        {
            public string FilePath { get; set; }
            public bool IsBinary { get; set; }
        }
    }
}

