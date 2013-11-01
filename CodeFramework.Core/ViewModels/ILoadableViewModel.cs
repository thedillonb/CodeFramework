using System.ComponentModel;
using System.Threading.Tasks;

namespace CodeFramework.Core.ViewModels
{
    public interface ILoadableViewModel : INotifyPropertyChanged
    {
        Task Load(bool forceDataRefresh);
    }
}

