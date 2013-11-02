using Cirrious.CrossCore;
using CodeFramework.Core.Services;

namespace CodeFramework.Core.ViewModels
{
    public class FilterableCollectionViewModel<T, TF> : CollectionViewModel<T>, IFilterableViewModel<TF> where TF : FilterModel<TF>, new()
    {
        protected TF _filter;
        private readonly string _filterKey;

        public TF Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                RaisePropertyChanged(() => Filter);
            }
        }

        public FilterableCollectionViewModel(string filterKey)
        {
            _filterKey = filterKey;
           // _filter = CodeFramework.Data.Accounts.Instance.ActiveAccount.GetFilter<TF>(_filterKey);
        }

        public void ApplyFilter(TF filter, bool saveAsDefault = false)
        {
            Filter = filter;
            // (saveAsDefault)
                //CodeFramework.Data.Accounts.Instance.ActiveAccount.AddFilter(_filterKey, filter);
        }
    }
}

