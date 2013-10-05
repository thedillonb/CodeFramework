using System;
using CodeFramework.Filters.Models;

namespace CodeFramework.Controllers
{
    public interface IFilterController<TFilter> where TFilter : FilterModel<TFilter>, new()
    {
        TFilter Filter { get; }

        void ApplyFilter(TFilter filter, bool saveAsDefault = false, bool render = true);
    }
}

