using System;

namespace CodeFramework.Controllers
{
    public interface IController
    {
        bool IsModelValid { get; }
        void Update(bool forceDataRefresh);
        void Refresh();
    }
}

