using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeFramework.Filters.Models
{
    public class FilterGroup<TElement> : IGrouping<string, TElement>
    {
        readonly List<TElement> _elements;

        public FilterGroup(string key, List<TElement> elements)
        {
            Key = key;
            _elements = elements;
        }

        public System.Collections.Generic.IEnumerator<TElement> GetEnumerator()
        {
            return this._elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Key { get; private set; }
    }
}

