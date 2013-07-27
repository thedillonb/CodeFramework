using System;
using CodeFramework.Elements;
using CodeFramework.Views;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch;
using System.Threading.Tasks;
using RedPlum;

namespace CodeFramework.Controllers
{
    public abstract class BaseController : BaseDialogViewController
    {
        public bool Loaded { get; private set; }
        public bool EnableFilter { get; set; }
        protected ErrorView CurrentError;
        private UISearchBar _searchBar;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
        protected BaseController(bool push = false, bool refresh = true)
            : base(push)
        {
            if (refresh)
                RefreshRequested += async (sender, e) => {
                    await Refresh(true);
                };
        }

        protected override UISearchBar CreateHeaderView()
        {
            if (EnableFilter)
            {
                var searchBar = new SearchFilterBar {Delegate = new CustomSearchDelegate(this)};
                searchBar.FilterButton.TouchUpInside += FilterButtonTouched;
                _searchBar = searchBar;
            }
            //There was no filter!
            else
            {
                _searchBar = new UISearchBar(new RectangleF(0f, 0f, 320f, 44f)) {Delegate = new CustomSearchDelegate(this)};
            }

            _searchBar.Placeholder = SearchPlaceholder;
            return _searchBar;
        }

        void FilterButtonTouched (object sender, EventArgs e)
        {
            var filter = CreateFilterController();
            if (filter != null)
                ShowFilterController(filter);
        }

        protected virtual FilterController CreateFilterController()
        {
            return null;
        }

        protected void AddItems<T>(List<T> items, Func<T, Element> createFunc)
        {
            var sec = new Section();
            if (items.Count == 0)
                sec.Add(new NoItemsElement());
            else
                foreach (var e in items)
                    sec.Add(createFunc(e));

            var r = new RootElement(Title) { UnevenRows = Root.UnevenRows };
            r.Add(sec);
            Root = r;
        }

        private void ShowFilterController(FilterController filter)
        {
            filter.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Images.Buttons.Cancel, () => { 
                filter.DismissViewController(true, null);
            }));
            filter.NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Images.Buttons.Save, () => {
                filter.DismissViewController(true, null); 
                filter.ApplyFilter();
            }));

            var nav = new UINavigationController(filter);
            PresentViewController(nav, true, null);
        }

        protected virtual async Task DoRefresh(bool force)
        {
            throw new NotImplementedException("You should implement this...");
        }

        protected async Task Refresh(bool force = false)
        {
            if (CurrentError != null)
                CurrentError.RemoveFromSuperview();
            CurrentError = null;

            if (force)
            {
                try
                {
                    Utilities.PushNetworkActive();
                    await DoRefresh(force);
                }
                catch (Exception ex)
                {
                    Utilities.ShowAlert("Unable to refresh!", "There was an issue while attempting to refresh. " + ex.Message);
                }
                finally
                {
                    Utilities.PopNetworkActive();
                }
            }
            else
            {
                MBProgressHUD hud = null;
                UIView parent = null;

                //Don't attach it to the UI window. It doesn't work well with orientation
                if (this.View.Superview is UIWindow)
                    parent = this.View;
                else
                    parent = this.View.Superview;

                hud = new MBProgressHUD(parent) {Mode = MBProgressHUDMode.Indeterminate, TitleText = "Loading..."};
                parent.AddSubview(hud);
                hud.Show(true);

                try
                {
                    Utilities.PushNetworkActive();
                    await DoRefresh(force);
                }
                catch (Exception ex)
                {
                    CurrentError = ErrorView.Show(View.Superview, ex.Message);
                }
                finally 
                {
                    Utilities.PopNetworkActive();
                }

                hud.Hide(true);
                hud.RemoveFromSuperview();
            }

            if (TableView.TableFooterView != null)
                TableView.TableFooterView.Hidden = Root.Count == 0;

            Loaded = true;
            ReloadComplete();
        }

        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!Loaded)
            {
                await Refresh();
                Loaded = true;
            }
        }

        protected virtual void SearchStart()
        {
            var searchBar = _searchBar as SearchFilterBar;
            if (searchBar != null)
                searchBar.FilterButtonVisible = false;
        }

        protected virtual void SearchEnd()
        {
            var searchBar = _searchBar as SearchFilterBar;
            if (searchBar != null)
                searchBar.FilterButtonVisible = true;
        }


        class CustomSearchDelegate : UISearchBarDelegate
        {
            readonly BaseController _container;
            DialogViewController _searchController;
            List<ElementContainer> _searchElements;

            static UIColor NoItemColor = UIColor.FromRGBA(0.1f, 0.1f, 0.1f, 0.9f);

            class ElementContainer
            {
                public Element Element;
                public Element Parent;
            }

            public CustomSearchDelegate (BaseController container)
            {
                _container = container;
            }

            public override void OnEditingStarted (UISearchBar searchBar)
            {
                _container.SearchStart();

                if (_searchController == null)
                {
                    _searchController = new DialogViewController(UITableViewStyle.Plain, null);
                    _searchController.LoadView();
                    _searchController.TableView.TableFooterView = new DropbarView(1f);
                }

                searchBar.ShowsCancelButton = true;
                _container.TableView.ScrollRectToVisible(new RectangleF(0, 0, 1, 1), false);
                _container.NavigationController.SetNavigationBarHidden(true, true);
                _container.IsSearching = true;
                _container.TableView.ScrollEnabled = false;

                if (_searchController.Root != null && _searchController.Root.Count > 0 && _searchController.Root[0].Count > 0)
                {
                    _searchController.TableView.TableFooterView.Hidden = false;
                    _searchController.View.BackgroundColor = UIColor.White;
                    _searchController.TableView.ScrollEnabled = true;
                }
                else
                {
                    _searchController.TableView.TableFooterView.Hidden = true;
                    _searchController.View.BackgroundColor = NoItemColor;
                    _searchController.TableView.ScrollEnabled = false;
                }

                _searchElements = new List<ElementContainer>();

                //Grab all the elements that we could search trhough
                foreach (var s in _container.Root)
                    foreach (var e in s.Elements)
                        _searchElements.Add(new ElementContainer { Element = e, Parent = e.Parent });

                if (!_container.ChildViewControllers.Contains(_searchController))
                {
                    _searchController.View.Frame = new RectangleF(_container.TableView.Bounds.X, 44f, _container.TableView.Bounds.Width, _container.TableView.Bounds.Height - 44f);
                    _container.AddChildViewController(_searchController);
                    _container.View.AddSubview(_searchController.View);
                }


            }

            public override void OnEditingStopped (UISearchBar searchBar)
            {

            }

            public override void TextChanged (UISearchBar searchBar, string searchText)
            {
                if (string.IsNullOrEmpty(searchText))
                {
                    if (_searchController.Root != null)
                        _searchController.Root.Clear();
                    _searchController.View.BackgroundColor = NoItemColor;
                    _searchController.TableView.TableFooterView.Hidden = true;
                    _searchController.TableView.ScrollEnabled = false;
                    return;
                }

                var sec = new Section();
                foreach (var el in _searchElements)
                {
                    if (el.Element.Matches(searchText))
                    {
                        sec.Add(el.Element);
                    }
                }
                _searchController.TableView.ScrollEnabled = true;

                if (sec.Count == 0)
                {
                    sec.Add(new NoItemsElement());
                }

                _searchController.View.BackgroundColor = UIColor.White;
                _searchController.TableView.TableFooterView.Hidden = sec.Count == 0;
                var root = new RootElement("") { sec };
                root.UnevenRows = true;
                _searchController.Root = root;
            }

            public override void CancelButtonClicked (UISearchBar searchBar)
            {
                //Reset the parent
                foreach (var s in _searchElements)
                    s.Element.Parent = s.Parent;

                searchBar.Text = "";
                searchBar.ShowsCancelButton = false;
                _container.FinishSearch ();
                searchBar.ResignFirstResponder ();
                _container.NavigationController.SetNavigationBarHidden(false, true);
                _container.IsSearching = false;
                _container.TableView.ScrollEnabled = true;

                _searchController.RemoveFromParentViewController();
                _searchController.View.RemoveFromSuperview();

                if (_searchController.Root != null)
                    _searchController.Root.Clear();

                _searchElements.Clear();
                _searchElements = null;

                _container.SearchEnd();
            }

            public override void SearchButtonClicked (UISearchBar searchBar)
            {
                //container.SearchButtonClicked (searchBar.Text);
                searchBar.ResignFirstResponder();


                //Enable the cancel button again....
                foreach (var s in searchBar.Subviews)
                {
                    var x = s as UIButton;
                    if (x != null)
                        x.Enabled = true;
                }
            }
        }

    }
}

