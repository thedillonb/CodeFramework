using System;

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
                    Console.WriteLine("FireAndForget failed: " + e.Message + " - " + e.StackTrace);
                }
            });

            t.Start();
        }
    }
}

