using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using CodeFramework.Filters.Models;

namespace CodeFramework.Controllers
{
    public abstract class ListController<T, TFilter> : Controller<ListModel<T>>, IFilterController<TFilter> where T : new() where TFilter : FilterModel<TFilter>, new()
    {
        public static int[] IntegerCeilings = new[] { 6, 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 251, 501, 1001, 2001, 4001, 8001, 16001, int.MaxValue };

        public TFilter Filter { get; protected set; }

        protected ListController(IView<ListModel<T>> view)
            : base(view)
        {
        }

        protected void RenderView(List<T> data, Action more)
        {
            RenderView(new ListModel<T>(data, more));
        }

        protected override void RenderView()
        {
            var viewData = new ListModel<T> {More = Model.More, Data = FilterModel(Model.Data, Filter)};
            viewData.FilteredData = GroupModel(viewData.Data, Filter);

            if (NSThread.IsMain)
                View.Render(viewData);
            else
                _object.BeginInvokeOnMainThread(() => View.Render(viewData));
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
                RenderView();
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

        protected void RenderView(List<T> data, Action more)
        {
            RenderView(new ListModel<T>(data, more));
        }
    }
}

