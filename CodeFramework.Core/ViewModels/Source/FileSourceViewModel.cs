using System;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeFramework.Core.ViewModels.Source
{
    public class FileSourceItemViewModel
    {
        public string FilePath { get; set; }
        public bool IsBinary { get; set; }
    }

    public interface IFileSourceViewModel : IBaseViewModel
    {
        FileSourceItemViewModel SourceItem { get; }

        string Theme { get; set; }

        IReactiveCommand<object> NextItemCommand { get; }

        IReactiveCommand<object> PreviousItemCommand { get; }
    }

    public abstract class FileSourceViewModel<TFileModel> : BaseViewModel, ILoadableViewModel, IFileSourceViewModel
    {
        private readonly TFileModel[] _items;

        public string Title { get; set; }

        public string HtmlUrl { get; set; }

        private FileSourceItemViewModel _source;
        public FileSourceItemViewModel SourceItem
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

        private int _currentItemIndex;
        public int CurrentItemIndex
        {
            get { return _currentItemIndex; }
            set { this.RaiseAndSetIfChanged(ref _currentItemIndex, value); }
        }

        ObservableAsPropertyHelper<TFileModel> _currentItem;
        public TFileModel CurrentItem
        {
            get { return _currentItem.Value; }
        }

        public IReactiveCommand<object> NextItemCommand { get; private set; }

        public IReactiveCommand<object> PreviousItemCommand { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        protected FileSourceViewModel(NavObject navObject)
        {
            _items = navObject.Items;
            _currentItemIndex = navObject.CurrentItemIndex;

            var hasNextItems = this.WhenAnyValue(y => y.CurrentItemIndex, y => _items != null && _items.Length > 1 && y < (_items.Length - 1));
            NextItemCommand = ReactiveCommand.Create(
                this.LoadCommand.CanExecuteObservable.CombineLatest(
                    hasNextItems, (canLoad, hasNext) => canLoad && hasNext)
                .DistinctUntilChanged()
                .StartWith(false));
            NextItemCommand.Subscribe(x => CurrentItemIndex += 1);

            var hasPreviousItems = this.WhenAnyValue(y => y.CurrentItemIndex, y => _items != null && _items.Length > 1 && y > 0);
            PreviousItemCommand = ReactiveCommand.Create(
                this.LoadCommand.CanExecuteObservable.CombineLatest(
                    hasPreviousItems, (canLoad, hasPrevious) => canLoad && hasPrevious)
                .DistinctUntilChanged()
                .StartWith(false));
            PreviousItemCommand.Subscribe(x => CurrentItemIndex -= 1);

            this.WhenAnyValue(x => x.CurrentItemIndex)
                .Where(x => _items != null && x < _items.Length)
                .Select(x => _items[x])
                .ToProperty(this, r => r.CurrentItem, out _currentItem, scheduler: System.Reactive.Concurrency.Scheduler.Immediate);
        }

        protected static string CreatePlainContentFile(string data, string filename)
        {
            var filepath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), filename);
            System.IO.File.WriteAllText(filepath, data, System.Text.Encoding.UTF8);
            return filepath;
        }

        public class NavObject
        {
            public int CurrentItemIndex { get; set; }
            public TFileModel[] Items { get; set; }
        }
    }
}

