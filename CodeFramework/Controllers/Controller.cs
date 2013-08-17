using System;
using System.Threading;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace CodeFramework.Controllers
{
    public interface IController
    {
        bool IsModelValid { get; }
        void Update(bool force);
        void UpdateAndRender(bool force);
        void Render();
    }
//
//    public abstract class ListController<T, F> : Controller<T, F> where T : new() where F : new()
//    {
//        public ListController(IView<ListModel<T>> view)
//            : base(view)
//        {
//        }
//    }
//
//    public abstract class ListController<T> : Controller<T> where T : new()
//    {
//        public ListController(IView<ListModel<T>> view)
//            : base(view)
//        {
//        }
//    }

    public abstract class Controller<T, F> : Controller<T> where T : new() where F : new()
    {
        public F Filter { get; set; }

        public Controller(IView<T> view)
            : base(view)
        {
        }

        protected abstract T FilterModel(T model, F filter);

        protected override void DoViewRender()
        {
            View.Render(FilterModel(Model, Filter));
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

    public class ListModel<T>
    {
        public T Data { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Action More { get; set; }
    }
}

