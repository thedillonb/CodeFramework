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
			{
				try
				{
                	b();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
        };

		if (activateNow)
		{
			try
			{
				b();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
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
			{
				try
				{
					b(comp(viewModel));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
        };

		if (activateNow)
		{
			try
			{
				b(comp(viewModel));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
    }

	public static void BindCollection<T>(this T viewModel, System.Linq.Expressions.Expression<Func<T, INotifyCollectionChanged>> outExpr, Action<NotifyCollectionChangedEventArgs> b, bool activateNow = false) where T : INotifyPropertyChanged
    {
        var exp = outExpr.Compile();
        var m = exp(viewModel);
		m.CollectionChanged += (sender, e) =>
		{
			try
			{
				b(e);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		};

		if (activateNow)
		{
			try
			{
				b(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
    }
}

