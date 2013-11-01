using System;

namespace CodeFramework.Core.ViewModels
{
    [Serializable]
    public abstract class FilterModel<F>
    {
        public abstract F Clone();
    }
}

