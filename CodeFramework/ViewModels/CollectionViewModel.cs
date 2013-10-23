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
        private int _deferLevel;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public CustomObservableCollection<TItem> Items
        {
            get { return _source; }
        }

        public bool IsDefering
        {
            get
            {
                return _deferLevel > 0;
            }
        }

        public Action MoreItems
        {
            get { return _moreItems; }
            set 
            {
                _moreItems = value;
                if (!IsDefering)
                    OnPropertyChanged(() => MoreItems);
            }
        }

        public Func<IEnumerable<TItem>, IEnumerable<TItem>> SortingFunction
        {
            get { return _sortingFunction; }
            set
            {
                _sortingFunction = value;
                if (!IsDefering)
                    OnPropertyChanged(() => SortingFunction);
            }
        }

        public Func<IEnumerable<TItem>, IEnumerable<TItem>> FilteringFunction
        {
            get { return _filteringFunction; }
            set
            {
                _filteringFunction = value;
                if (!IsDefering)
                    OnPropertyChanged(() => FilteringFunction);
            }
        }

        public Func<IEnumerable<TItem>, IEnumerable<IGrouping<string, TItem>>> GroupingFunction
        {
            get { return _groupingFunction; }
            set
            {
                _groupingFunction = value;
                if (!IsDefering)
                    OnPropertyChanged(() => GroupingFunction);
            }
        }

        public CollectionViewModel()
            : this (new CustomObservableCollection<TItem>())
        {
        }

        public CollectionViewModel(CustomObservableCollection<TItem> source)
        {
            //Forward events
            _source = source;
            _source.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => 
            {
                if (IsDefering)
                    return;

                var eventHandler = CollectionChanged;
                if (eventHandler != null)
                    eventHandler(sender, e);
            };
        }

        public void Refresh()
        {
            if (IsDefering)
                return;

            var eventHandler = CollectionChanged;
            if (eventHandler != null)
                eventHandler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IDisposable DeferRefresh()
        {
            ++_deferLevel;
            return new DeferHelper(this);
        }

        private void EndDefer()
        {
            --_deferLevel;
            if (_deferLevel == 0)
                Refresh();
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class DeferHelper : IDisposable
        {
            private CollectionViewModel<TItem> _parent;

            public DeferHelper(CollectionViewModel<TItem> parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
                if (_parent != null)
                    _parent.EndDefer();
            }
        }
    }
}

