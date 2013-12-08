using Cirrious.CrossCore;
using CodeFramework.Core.Services;

namespace System.Threading.Tasks
{
    public static class FireAndForgetTask
    {
        public static void Start(Action action)
        {
            var t = new Task(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Mvx.Resolve<IErrorReporter>().ReportError("FireAndForget failed", e);
                }
            });

            t.Start();
        }
    }
}

