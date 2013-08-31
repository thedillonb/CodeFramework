namespace CodeFramework.Filters.Models
{
    public abstract class FilterModel<TFilter>
    {
        public abstract TFilter Clone();
    }
}

