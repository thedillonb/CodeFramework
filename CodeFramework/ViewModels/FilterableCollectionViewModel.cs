using System;
using CodeFramework.Filters.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeFramework.ViewModels
{
    public abstract class FilterableCollectionViewModel<T, F> : CollectionViewModel<T>, IFilterableViewModel<F> where F : FilterModel<F>, new()
    {
        public static int[] IntegerCeilings = new[] { 6, 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 251, 501, 1001, 2001, 4001, 8001, 16001, int.MaxValue };

        protected F _filter;
        private readonly string _filterKey;

        public F Filter
        {
            get { return _filter; }
            set { SetProperty(ref _filter, value); }
        }

        protected FilterableCollectionViewModel(string filterKey)
        {
            _filterKey = filterKey;
            _filter = CodeFramework.Data.Accounts.Instance.ActiveAccount.GetFilter<F>(_filterKey);

            this.PropertyChanged += (sender, e) => {
                if (e.PropertyName.Equals("Filter"))
                    FilterChanged();
            };
        }

        public void ApplyFilter(F filter, bool saveAsDefault = false)
        {
            Filter = filter;
            if (saveAsDefault)
                CodeFramework.Data.Accounts.Instance.ActiveAccount.AddFilter(_filterKey, filter);
        }

        protected abstract void FilterChanged();

        private static string CreateRangeString(int key)
        {
            return IntegerCeilings.LastOrDefault(x => x < key) + " to " + (key - 1);
        }

        protected static List<IGrouping<string, TElement>> CreateNumberedGroup<TElement>(IEnumerable<IGrouping<int, TElement>> results, string title, string prefix = null)
        {
            return results.Select(x => {
                var text = (prefix != null ? prefix + " " : "") + CreateRangeString(x.Key) + " " + title;
                return (IGrouping<string, TElement>)new FilterGroup<TElement>(text, x.ToList());
            }).ToList();
        }
    }
}

