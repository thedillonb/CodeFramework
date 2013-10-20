using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CodeFramework.ViewModels
{
    public interface ILoadableViewModel : INotifyPropertyChanged
    {
        Task Load(bool forceDataRefresh);
    }
}

