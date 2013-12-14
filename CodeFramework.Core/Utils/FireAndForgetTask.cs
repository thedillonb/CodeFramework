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
						e.Report();
                }
            });

            t.Start();
        }
    }
}

