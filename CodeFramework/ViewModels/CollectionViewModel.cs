using System;
using CodeFramework.Utils;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

namespace CodeFramework.ViewModels
{
    public class CollectionViewModel<TItem> : ViewModel, IEnumerable<TItem>, INotifyCollectionChanged
    {
        private readonly CustomObservableCollection<TItem> _source = new CustomObservableCollection<TItem>();
        private Func<IEnumerable<TItem>, IEnumerable<IGrouping<string, TItem>>> _groupingFunction;
        private Func<IEnumerable<TItem>, IEnumerable<TItem>> _sortingFunction;
        private Func<IEnumerable<TItem>, IEnumerable<TItem>> _filteringFunction;
        private Action _moreItems;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public CustomObservableCollection<TItem> Items
        {
            get { return _source; }
        }

        public Action MoreItems
        {
            get { return _moreItems; }
            set { SetProperty(ref _moreItems, value); }
        }

        public Func<IEnumerable<TItem>, IEnumerable<TItem>> SortingFunction
        {
            get { return _sortingFunction; }
            protected set { SetProperty(ref _sortingFunction, value); }
        }

        public Func<IEnumerable<TItem>, IEnumerable<TItem>> FilteringFunction
        {
            get { return _filteringFunction; }
            protected set { SetProperty(ref _filteringFunction, value); }
        }

        public Func<IEnumerable<TItem>, IEnumerable<IGrouping<string, TItem>>> GroupingFunction
        {
            get { return _groupingFunction; }
            protected set { SetProperty(ref _groupingFunction, value); }
        }

        public CollectionViewModel()
        {
            //Forward events
            _source.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
                var eventHandler = CollectionChanged;
                if (eventHandler != null)
                    eventHandler(sender, e);
            };
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

