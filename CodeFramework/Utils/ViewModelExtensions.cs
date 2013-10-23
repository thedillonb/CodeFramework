using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using System.Collections.Specialized;
using MonoTouch.Dialog;
using CodeFramework.ViewModels;
using System.Collections.Generic;
using System.Linq;

public static class ViewModelExtensions
{
    private static NSObject uiObject = new NSObject();

    public static void Bind<T, R>(this T viewModel, System.Linq.Expressions.Expression<Func<T, R>> outExpr, Action b) where T : CodeFramework.ViewModels.ViewModel
    {
        INotifyPropertyChanged m = viewModel;
        var expr = (System.Linq.Expressions.MemberExpression) outExpr.Body;
        var prop = (System.Reflection.PropertyInfo) expr.Member;
        var name = prop.Name;
        m.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
            if (e.PropertyName.Equals(name))
                uiObject.BeginInvokeOnMainThread(new MonoTouch.Foundation.NSAction(b));
        };
    }

    public static void Bind<T, R>(this T viewModel, System.Linq.Expressions.Expression<Func<T, R>> outExpr, Action<R> b) where T : CodeFramework.ViewModels.ViewModel
    {
        INotifyPropertyChanged m = viewModel;
        var expr = (System.Linq.Expressions.MemberExpression) outExpr.Body;
        var prop = (System.Reflection.PropertyInfo) expr.Member;
        var name = prop.Name;
        var comp = outExpr.Compile();
        m.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
            if (e.PropertyName.Equals(name))
                uiObject.BeginInvokeOnMainThread(new MonoTouch.Foundation.NSAction(() => b(comp(viewModel))));
        };
    }

    public static void BindCollection<T>(this T viewModel, System.Linq.Expressions.Expression<Func<T, INotifyCollectionChanged>> outExpr, Action<NotifyCollectionChangedEventArgs> b) where T : CodeFramework.ViewModels.ViewModel
    {
        var exp = outExpr.Compile();
        INotifyCollectionChanged m = exp(viewModel);
        m.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
            uiObject.BeginInvokeOnMainThread(() => b(e));
        };
    }
}

