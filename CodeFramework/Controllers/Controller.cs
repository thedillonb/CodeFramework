using System;
using System.Threading;
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

    public interface IFilterController<F> where F : FilterModel<F>, new()
    {
        F Filter { get; }

        void ApplyFilter(F filter, bool saveAsDefault = false, bool render = true);
    }

    public abstract class ListController<T, F> : Controller<ListModel<T>>, IFilterController<F> where T : new() where F : FilterModel<F>, new()
    {
        public static int[] IntegerCeilings = new[] { 6, 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 251, 501, 1001, 2001, 4001, 8001, 16001, int.MaxValue };

        public F Filter { get; protected set; }

        public ListController(IView<ListModel<T>> view)
            : base(view)
        {
        }

        protected override void DoViewRender()
        {
            var viewData = new ListModel<T>() { More = Model.More };
            viewData.Data = FilterModel(Model.Data, Filter);
            viewData.FilteredData = GroupModel(viewData.Data, Filter);
            View.Render(viewData);
        }

        protected virtual List<T> FilterModel(List<T> model, F filter)
        {
            return model;
        }

        protected virtual List<IGrouping<string, T>> GroupModel(List<T> model, F filter)
        {
            return null;
        }

        protected abstract void SaveFilterAsDefault(F filter);

        public virtual void ApplyFilter(F filter, bool saveAsDefault = false, bool render = true)
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
        public ListController(IView<ListModel<T>> view)
            : base(view)
        {
        }
    }
    
    public abstract class Controller<T> : IController where T : new()
    {
        private static NSObject _object = new NSObject();

        public IView<T> View { get; private set; }

        public Controller(IView<T> view)
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
            _object.InvokeOnMainThread(DoViewRender);
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

