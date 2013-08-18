using System;

namespace CodeFramework.Filters.Models
{
    public abstract class FilterModel<F>
    {
        public abstract F Clone();
    }
}

