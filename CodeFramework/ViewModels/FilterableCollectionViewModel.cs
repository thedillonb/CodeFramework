using System;
using CodeFramework.Filters.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeFramework.ViewModels
{
    public class FilterableCollectionViewModel<T, F> : CollectionViewModel<T>, IFilterableViewModel<F> where F : FilterModel<F>, new()
    {
        protected F _filter;
        private readonly string _filterKey;

        public F Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value); }
        }

        public FilterableCollectionViewModel(string filterKey)
        {
            _filterKey = filterKey;
            _filter = CodeFramework.Data.Accounts.Instance.ActiveAccount.GetFilter<F>(_filterKey);
        }

        public void ApplyFilter(F filter, bool saveAsDefault = false)
        {
            Filter = filter;
            if (saveAsDefault)
                CodeFramework.Data.Accounts.Instance.ActiveAccount.AddFilter(_filterKey, filter);
        }
    }
}

