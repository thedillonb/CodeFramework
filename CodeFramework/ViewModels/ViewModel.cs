using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CodeFramework.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            //if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            var expr = (System.Linq.Expressions.MemberExpression) property.Body;
            var prop = (System.Reflection.PropertyInfo) expr.Member;
            OnPropertyChanged(prop.Name);
        }
    }
}

