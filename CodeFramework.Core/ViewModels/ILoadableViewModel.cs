using System.ComponentModel;
using System.Windows.Input;

namespace CodeFramework.Core.ViewModels
{
    public interface ILoadableViewModel
    {
        ICommand LoadCommand { get; }
    }
}

