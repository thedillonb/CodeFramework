using System.Collections.Generic;
using CodeFramework.Elements;
using MonoTouch.UIKit;
using System.Linq;
using System;
using CodeFramework.Controllers;
using CodeFramework.Filters.Models;
using CodeFramework.Views;

namespace CodeFramework.Filters.Controllers
{
    public abstract class FilterViewController : BaseDialogViewController
    {
        public FilterViewController()
            : base(true)
        {
            Title = "Filter & Sort".t();
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Images.Buttons.Cancel, () => { 
                DismissViewController(true, null);
            }));
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Images.Buttons.Save, () => {
                DismissViewController(true, null); 
                ApplyButtonPressed();
            }));
        }

        public abstract void ApplyButtonPressed();

        public void CloseViewController()
        {
            DismissViewController(true, null);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TableView.ReloadData();
        }

        public class EnumChoiceElement : MonoTouch.Dialog.StyledStringElement
        {
            public int Obj;
            public EnumChoiceElement(string title, string defaultVal, IEnumerable<string> values)
                : base(title, defaultVal, UITableViewCellStyle.Value1)
            {
                Accessory = UITableViewCellAccessory.DisclosureIndicator;
                int i = 0;
                foreach (var a in values)
                {
                    if (a.Equals(defaultVal))
                    {
                        Obj = i;
                        break;
                    }
                    i++;
                }
            }
        }

        protected EnumChoiceElement CreateEnumElement(string title, string defaultVal, IEnumerable<string> values)
        {
            var element = new EnumChoiceElement(title, defaultVal, values);
            element.Tapped += () =>
            {
                var en = new RadioChoiceViewController(element.Caption, values, element.Value);
                en.ValueSelected += obj =>
                {
                    element.Value = obj;

                    int i = 0;
                    foreach (var a in values)
                    {
                        if (a.Equals(obj))
                        {
                            element.Obj = i;
                            break;
                        }
                        i++;
                    }

                    NavigationController.PopViewControllerAnimated(true);
                };
                NavigationController.PushViewController(en, true);
            };
            
            return element;
        }

        protected EnumChoiceElement CreateEnumElement(string title, int defaultVal, System.Type enumType)
        {
            var values = new List<string>();
            foreach (var x in System.Enum.GetValues(enumType).Cast<System.Enum>())
                values.Add(x.Description());

            return CreateEnumElement(title, values[defaultVal], values);
        }

        public class MultipleChoiceElement<T> : MonoTouch.Dialog.StyledStringElement
        {
            public T Obj;
            public MultipleChoiceElement(string title, T obj)
                : base(title, CreateCaptionForMultipleChoice(obj), UITableViewCellStyle.Value1)
            {
                Obj = obj;
                Accessory = UITableViewCellAccessory.DisclosureIndicator;
            }
        }

        protected MultipleChoiceElement<T> CreateMultipleChoiceElement<T>(string title, T o)
        {
            var element = new MultipleChoiceElement<T>(title, o);
            element.Tapped += () =>
            {
                var en = new MultipleChoiceViewController(element.Caption, o);
                en.ViewDisappearing += (sender, e) => {
                    element.Value = CreateCaptionForMultipleChoice(o);
                };
                NavigationController.PushViewController(en, true);
            };

            return element;
        }

        private static string CreateCaptionForMultipleChoice<T>(T o)
        {
            var fields = o.GetType().GetFields();
            var sb = new System.Text.StringBuilder();
            int trueCounter = 0;
            foreach (var f in fields)
            {
                if ((bool)f.GetValue(o))
                {
                    sb.Append(f.Name);
                    sb.Append(", ");
                    trueCounter++;
                }
            }
            var str = sb.ToString();
            if (str.EndsWith(", "))
            {
                if (trueCounter == fields.Length)
                    return "Any".t();
                else
                    return str.Substring(0, str.Length - 2);
            }
            return "None".t();
        }
    }
}

