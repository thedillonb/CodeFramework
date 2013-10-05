using System;

namespace CodeFramework.Controllers
{
    public interface IView<T>
    {
        void Render(T model);

        void ShowLoading(bool background, Action loadAction);
    }

    public interface IListView<T> : IView<ListModel<T>>
    {
    }
}

