using System;
using CodeFramework.Elements;
using MonoTouch.Dialog;
using MonoTouch;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Collections;

namespace CodeFramework.Controllers
{
    public abstract class BaseListModelController : BaseModelDrivenController
    {
        private int _nextPage = 0;
        private LoadMoreElement _loadMore;

        public string NoItemsText { get; set; }

        protected BaseListModelController(bool push = false, bool refresh = true)
            : base(push, refresh)
        {
            Style = UITableViewStyle.Plain;
            EnableSearch = true;
        }
        
        protected abstract object OnUpdateListModel(bool forced, int currentPage, ref int nextPage);

        protected abstract Element CreateElement(object obj);

        protected sealed override object OnUpdateModel(bool forced)
        {
            _nextPage = 0;
            return OnUpdateListModel(forced, _nextPage, ref _nextPage);
        }

        protected virtual IList OnRenderList()
        {
            return Model as IList;
        }
        
        protected override void OnRender()
        {
            var sec = new Section();
            var l = OnRenderList();

            if (l == null || l.Count == 0)
                sec.Add(NoItemsText == null ? new NoItemsElement() : new NoItemsElement(NoItemsText));
            else
            {
                foreach (var e in l)
                    sec.Add(CreateElement(e));
            }

            Section loadSec = null;
            if (_nextPage > 0)
            {
                _loadMore = new PaginateElement("Load More".t(), "Loading...".t(), e => this.DoWorkNoHud(LoadWork, LoadException, LoadFinished)) { AutoLoadOnVisible = true };
                loadSec = new Section() { _loadMore };
            }

            var r = new RootElement(Title) { UnevenRows = Root.UnevenRows };
            r.Add(sec);
            if (loadSec != null)
                r.Add(loadSec);
            Root = r;
        }

        private void LoadWork()
        {
            var data = OnUpdateListModel(true, _nextPage, ref _nextPage) as IList;

            if (Model == null)
                Model = data;
            else
            {
                var l = Model as IList;
                foreach (var e in data)
                    l.Add(e);
            }

            InvokeOnMainThread(Render);
        }

        private void LoadException(Exception ex)
        {
            Utilities.ShowAlert("Failure to load!".t(), "Unable to load additional enries because the following error: ".t() + ex.Message);
        }

        private void LoadFinished()
        {
            if (_loadMore != null)
            {
                _loadMore.Animating = false;
                if (_nextPage < 0)
                {
                    Root.Remove(_loadMore.Parent as Section);
                    _loadMore.Dispose();
                    _loadMore = null;
                }
            }
        }
    }
}

