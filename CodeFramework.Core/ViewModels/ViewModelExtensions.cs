using System;
using System.ComponentModel;
using System.Collections.Specialized;

public static class ViewModelExtensions
{
	public static void Bind<T, TR>(this T viewModel, System.Linq.Expressions.Expression<Func<T, TR>> outExpr, Action b, bool activateNow = false) where T : INotifyPropertyChanged
    {
        var expr = (System.Linq.Expressions.MemberExpression) outExpr.Body;
        var prop = (System.Reflection.PropertyInfo) expr.Member;
        var name = prop.Name;
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName.Equals(name))
                b();
        };

		if (activateNow)
			b();
    }

	public static void Bind<T, TR>(this T viewModel, System.Linq.Expressions.Expression<Func<T, TR>> outExpr, Action<TR> b, bool activateNow = false) where T : INotifyPropertyChanged
    {
        var expr = (System.Linq.Expressions.MemberExpression) outExpr.Body;
        var prop = (System.Reflection.PropertyInfo) expr.Member;
        var name = prop.Name;
        var comp = outExpr.Compile();
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName.Equals(name))
                b(comp(viewModel));
        };

		if (activateNow)
			b(comp(viewModel));
    }

    public static void BindCollection<T>(this T viewModel, System.Linq.Expressions.Expression<Func<T, INotifyCollectionChanged>> outExpr, Action<NotifyCollectionChangedEventArgs> b) where T : INotifyPropertyChanged
    {
        var exp = outExpr.Compile();
        var m = exp(viewModel);
        m.CollectionChanged += (sender, e) => b(e);
    }
}

