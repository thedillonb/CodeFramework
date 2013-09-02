using System;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.Filters.Models;

namespace CodeFramework.Controllers
{
    public interface IController
    {
        bool IsModelValid { get; }
        void Update(bool force);
        void UpdateAndRender(bool force);
        void Render();
    }

    public interface IFilterController<TFilter> where TFilter : FilterModel<TFilter>, new()
    {
        TFilter Filter { get; }

        void ApplyFilter(TFilter filter, bool saveAsDefault = false, bool render = true);
    }

    public abstract class ListController<T, TFilter> : Controller<ListModel<T>>, IFilterController<TFilter> where T : new() where TFilter : FilterModel<TFilter>, new()
    {
        public static int[] IntegerCeilings = new[] { 6, 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 251, 501, 1001, 2001, 4001, 8001, 16001, int.MaxValue };

        public TFilter Filter { get; protected set; }

        protected ListController(IView<ListModel<T>> view)
            : base(view)
        {
        }

        protected override void DoViewRender()
        {
            var viewData = new ListModel<T> {More = Model.More, Data = FilterModel(Model.Data, Filter)};
            viewData.FilteredData = GroupModel(viewData.Data, Filter);
            View.Render(viewData);
        }

        protected virtual List<T> FilterModel(List<T> model, TFilter filter)
        {
            return model;
        }

        protected virtual List<IGrouping<string, T>> GroupModel(List<T> model, TFilter filter)
        {
            return null;
        }

        protected abstract void SaveFilterAsDefault(TFilter filter);

        public virtual void ApplyFilter(TFilter filter, bool saveAsDefault = false, bool render = true)
        {
            Filter = filter;
            if (saveAsDefault)
                SaveFilterAsDefault(filter);
            if (render)
                Render();
        }

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

    public abstract class ListController<T> : Controller<ListModel<T>> where T : new()
    {
        protected ListController(IView<ListModel<T>> view)
            : base(view)
        {
        }
    }
    
    public abstract class Controller<T> : IController where T : class, new()
    {
        private static NSObject _object = new NSObject();

        public IView<T> View { get; private set; }

        protected Controller(IView<T> view)
        {
            View = view;
        }

        public T Model { get; set; }

        public abstract void Update(bool force);

        public void UpdateAndRender(bool force)
        {
            Update(force);
            Render();
        }

        public void Render()
        {
            Exception ex = null;
            _object.InvokeOnMainThread(() => {
                try
                {
                    DoViewRender();
                } 
                catch (Exception e)
                {
                    ex = e;
                }
            });

            if (ex != null)
                throw ex;
        }

        protected virtual void DoViewRender()
        {
            View.Render(Model);
        }
        
        public bool IsModelValid 
        {
            get { return Model != null; }
        }
    }

    public interface IView<T>
    {
        void Render(T model);
    }

    public interface IListView<T> : IView<ListModel<T>>
    {
    }

    public class ListModel<T>
    {
        public List<T> Data { get; set; }
        public List<IGrouping<string, T>> FilteredData { get; set; }
        public Action More { get; set; }
    }
}

