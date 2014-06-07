using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using ReactiveUI;
using Xamarin.Utilities.Core.ReactiveAddons;

namespace CodeFramework.iOS.Views
{
    public abstract class ViewModelCollectionView<TViewModel> : ViewModelDialogView<TViewModel> where TViewModel : ReactiveObject
    {
        public string NoItemsText { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected ViewModelCollectionView()
        {
            NoItemsText = "No Items";
            Style = UITableViewStyle.Plain;
            EnableSearch = true;
        }

        protected void Bind<T>(IObservable<ReactiveCollection<T>> observableCollection, Func<T, Element> element)
        {
            observableCollection.Subscribe(x => x.Changed.Subscribe(_ =>
            {
                IEnumerable<T> items = x;
                if (x.FilterFunc != null)
                    items = items.Where(x.FilterFunc);
                if (x.OrderFunc != null)
                    items = items.OrderBy(x.OrderFunc);
                if (x.GroupFunc != null)
                {
                    RenderGroupedItems(items.GroupBy(x.GroupFunc), element, x.MoreTask);
                }
                else
                {
                    RenderList(items, element, x.MoreTask);
                }
            }));
        }

        protected void RenderList<T>(IEnumerable<T> items, Func<T, Element> select, Task moreAction)
        {
            var sec = new Section();
            if (items != null)
            {
                foreach (var item in items.ToList())
                {
                    try
                    {
                        var element = select(item);
                        if (element != null)
                            sec.Add(element);
                    }
                    catch (Exception e)
                    {
                        e.Report();
                    }
                }
            }

            RenderSections(new [] { sec }, moreAction);
        }

        protected virtual Section CreateSection(string text)
        {
            return new Section(text);
        }

        protected void RenderGroupedItems<T>(IEnumerable<IGrouping<object, T>> items, Func<T, Element> select, Task moreAction)
        {
            var sections = new List<Section>();

            if (items != null)
            {
                foreach (var grp in items.ToList())
                {
                    try
                    {
                        var sec = CreateSection(grp.Key.ToString());
                        foreach (var element in grp.Select(select).Where(element => element != null))
                            sec.Add(element);

                        if (sec.Elements.Count > 0)
                            sections.Add(sec);
                    }
                    catch (Exception e)
                    {
                        e.Report();
                    }
                }
            }

            RenderSections(sections, moreAction);
        }

        private void RenderSections(IEnumerable<Section> sections, Task moreAction)
        {
            var root = new RootElement(Title) { UnevenRows = Root.UnevenRows };

            foreach (var section in sections)
                root.Add(section);

            var elements = root.Sum(s => s.Elements.Count);

            //There are no items! We must have filtered them out
            if (elements == 0)
                root.Add(new Section { new NoItemsElement(NoItemsText) });

            if (moreAction != null)
            {
                var loadMore = new PaginateElement("Load More", "Loading...") { AutoLoadOnVisible = true };
                root.Add(new Section { loadMore });
                loadMore.Tapped += async (obj) =>
                {
                    try
                    {
                        await moreAction;
                        if (loadMore.GetImmediateRootElement() != null)
                        {
                            var section = loadMore.Parent as Section;
                            Root.Remove(section, UITableViewRowAnimation.Fade);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Unable to load more!", e.Message);
                    }
                };    
            }

            Root = root;
        }
    }
}

