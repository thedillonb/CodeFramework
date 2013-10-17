using System;

namespace CodeFramework.Filters.Models
{
    [Serializable]
    public abstract class FilterModel<F>
    {
        public abstract F Clone();
    }
}

