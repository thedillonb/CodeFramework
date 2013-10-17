using System;
using CodeFramework.Utils;
using System.Threading.Tasks;

namespace CodeFramework.ViewModels
{
    public abstract class SimpleCollectionViewModel<T> : ViewModel
    {
        private readonly CustomObservableCollection<T> _items;
        private Action _moreItemsTask;

        public CustomObservableCollection<T> Items
        {
            get { return _items; }
        }

		public Action MoreItems
        {
            get { return _moreItemsTask; }
            set { SetProperty(ref _moreItemsTask, value); }
        }

        protected SimpleCollectionViewModel()
        {
            _items = new CustomObservableCollection<T>();
        }
    }
}

