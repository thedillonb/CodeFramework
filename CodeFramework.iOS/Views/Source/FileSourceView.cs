using System;
using Xamarin.Utilities.ViewControllers;
using CodeFramework.Core.ViewModels.Source;
using ReactiveUI;
using System.Reactive.Linq;
using CodeFramework.iOS.SourceBrowser;
using Xamarin.Utilities.Views;
using System.Linq;

namespace CodeFramework.iOS.Views.Source
{
    public abstract class FileSourceView<TViewModel> : WebView<TViewModel> where TViewModel : FileSourceViewModel
    {
        protected FileSourceView()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.WhenAnyValue(x => x.Theme, x => new { Theme = x, ViewModel.SourceItem })
                .Skip(1)
                .Where(x => x != null && x.SourceItem != null && !x.SourceItem.IsBinary)
                .Subscribe(x => LoadContent(x.SourceItem.FilePath));

            ViewModel.WhenAnyValue(x => x.SourceItem)
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    if (x.IsBinary)
                    {
                        LoadFile(x.FilePath);
                    }
                    else
                    {
                        LoadContent(x.FilePath);
                    }
                });
        }

        private new void LoadContent(string filePath)
        {
            var content = System.IO.File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            var razorView = new SyntaxHighlighterView 
            { 
                Model = new SourceBrowserModel
                {
                    Content = content,
                    Theme = ViewModel.Theme ?? "idea"
                }
            };

            base.LoadContent(razorView.GenerateString());
        }

        protected void ShowThemePicker()
        {
            var themes = System.IO.Directory.GetFiles("SourceBrowser/styles")
                .Where(x => x.EndsWith(".css", StringComparison.Ordinal))
                .Select(x => System.IO.Path.GetFileNameWithoutExtension(x))
                .ToList();

            var selected = themes.IndexOf(ViewModel.Theme ?? "idea");
            if (selected <= 0)
                selected = 0;

            new PickerAlertView(themes.ToArray(), selected, x =>
            {
                if (x < themes.Count)
                    ViewModel.Theme = themes[x];
            }).Show();
        }
    }
}

