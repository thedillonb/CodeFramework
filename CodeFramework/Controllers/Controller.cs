using System;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.Filters.Models;
using System.Threading.Tasks;

namespace CodeFramework.Controllers
{
    public abstract class Controller<T> : IController where T : class, new()
    {
        protected static NSObject _object = new NSObject();

        public IView<T> View { get; private set; }

        public T Model { get; set; }

        protected Controller(IView<T> view)
        {
            View = view;
        }

        public void Update(bool forceDataRefresh)
        {
            View.ShowLoading(forceDataRefresh, () => OnUpdate(forceDataRefresh));
        }

        protected abstract void OnUpdate(bool forceDataRefresh);

        protected void RenderView(T model)
        {
            Model = model;
            RenderView();
        }

        protected virtual void RenderView()
        {
            if (NSThread.IsMain)
                View.Render(Model);
            else
                _object.BeginInvokeOnMainThread(() => View.Render(Model));
        }

        /// <summary>
        /// Refresh the controller and the view.
        /// </summary>
        public virtual void Refresh()
        {
            RenderView();
        }
        
        public bool IsModelValid 
        {
            get { return Model != null; }
        }
    }
}

