using System;
using System.Linq;
using CodeFramework.Views;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using CodeFramework.Filters.ViewControllers;
using CodeFramework.ViewModels;

namespace CodeFramework.ViewControllers
{
	public abstract class ViewModelCollectionDrivenViewController : ViewModelDrivenViewController
    {
        private bool _enableFilter;

        public string NoItemsText { get; set; }

        public bool EnableFilter
        {
            get { return _enableFilter; }
            set 
            {
                if (value)
                    EnableSearch = true;
                _enableFilter = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
		protected ViewModelCollectionDrivenViewController(bool push = true, bool refresh = true)
            : base(push, refresh)
        {
            NoItemsText = "No Items".t();
            Style = UITableViewStyle.Plain;
            EnableSearch = true;
        }

        protected void BindCollection<T, R>(T viewModel, Func<T, CollectionViewModel<R>> outExpr, Func<R, Element> element) where T : CodeFramework.ViewModels.ViewModel
        {
            BindCollection(outExpr(viewModel), element);
        }

        protected void BindCollection<TElement>(CollectionViewModel<TElement> viewModel, 
                                                Func<TElement, Element> element)
        {
            Action updateDel = () =>
            {
                IEnumerable<TElement> items = viewModel.Items;
                var filterFn = viewModel.FilteringFunction;
                if (filterFn != null)
                    items = filterFn(items);

                var sortFn = viewModel.SortingFunction;
                if (sortFn != null)
                    items = sortFn(items);

                var groupingFn = viewModel.GroupingFunction;
                IEnumerable<IGrouping<string, TElement>> groupedItems = null;
                if (groupingFn != null)
                    groupedItems = groupingFn(items);

                if (groupedItems == null)
                    RenderList(viewModel.Items, element, viewModel.MoreItems);
                else
                    RenderGroupedItems(groupedItems, element, viewModel.MoreItems);
            };

            Bind(viewModel, x => x.GroupingFunction, updateDel);
            Bind(viewModel, x => x.FilteringFunction, updateDel);
            Bind(viewModel, x => x.SortingFunction, updateDel);

            viewModel.Items.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
                BeginInvokeOnMainThread(() => updateDel());
            };
        }

        protected void RenderList<T>(IEnumerable<T> items, Func<T, Element> select, Action moreTask)
        {
            var sec = new Section();
            foreach (var item in items)
            {
                var element = select(item);
                if (element != null)
                    sec.Add(element);
            }

            RenderSections(new [] { sec }, moreTask);
        }

        protected void RenderGroupedItems<T>(IEnumerable<IGrouping<string, T>> items, Func<T, Element> select, Action moreTask)
        {
            var sections = new List<Section>(items.Count());
            foreach (var grp in items)
            {
                var sec = new Section(new TableViewSectionView(grp.Key));
                foreach (var element in grp.Select(select).Where(element => element != null))
                    sec.Add(element);

                if (sec.Elements.Count > 0)
                    sections.Add(sec);
            }

            RenderSections(sections, moreTask);
        }

        private void RenderSections(IEnumerable<Section> sections, Action moreTask)
        {
            var root = new RootElement(Title) { UnevenRows = Root.UnevenRows };

            foreach (var section in sections)
                root.Add(section);

            var elements = 0;
            foreach (var s in root)
                elements += s.Elements.Count;

            //There are no items! We must have filtered them out
            if (elements == 0)
                root.Add(new Section { new NoItemsElement(NoItemsText) });

            if (moreTask != null)
            {
                var loadMore = new PaginateElement("Load More".t(), "Loading...".t(), e => this.DoWorkNoHud(() => moreTask(),
                                                                                                            x => Utilities.ShowAlert("Unable to load more!".t(), x.Message))) { AutoLoadOnVisible = true };
                root.Add(new Section { loadMore });
            }

            Root = root;

            if (TableView.TableFooterView != null)
                TableView.TableFooterView.Hidden = false;
        }

        protected override void SearchStart()
        {
            base.StartSearch();

            var searchBar = SearchBar as SearchFilterBar;
            if (searchBar != null)
                searchBar.FilterButtonVisible = false;
        }

        protected override void SearchEnd()
        {
            var searchBar = SearchBar as SearchFilterBar;
            if (searchBar != null)
                searchBar.FilterButtonVisible = true;
        }

        protected override UISearchBar CreateSearchBar()
        {
            if (EnableFilter)
            {
                var searchBar = new SearchFilterBar { Delegate = new CustomSearchDelegate(this) };
                searchBar.FilterButton.TouchUpInside += FilterButtonTouched;
                return searchBar;
            }
            return base.CreateSearchBar();
        }

        protected virtual FilterViewController CreateFilterController()
        {
            return null;
        }

        void FilterButtonTouched (object sender, EventArgs e)
        {
            var filter = CreateFilterController();
            if (filter != null)
                ShowFilterController(filter);
        }

        private void ShowFilterController(FilterViewController filter)
        {
            var nav = new UINavigationController(filter);
            PresentViewController(nav, true, null);
        }
    }
}

