using System;

namespace CodeFramework.Filters.Models
{
    /// <summary>
    /// This is just a empty filter. Used more as a place holder.
    /// </summary>
    public class EmptyFilterModel : FilterModel<EmptyFilterModel>
    {
        public override EmptyFilterModel Clone()
        {
            return (EmptyFilterModel)this.MemberwiseClone();
        }

        public EmptyFilterModel()
        {
        }
    }
}

