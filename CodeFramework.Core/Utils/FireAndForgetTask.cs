
namespace System.Threading.Tasks
{
    public static class FireAndForgetTask
    {
		public static void FireAndForget(this Task task)
		{
			task.ContinueWith(t =>
			{
				var aggException = t.Exception.Flatten();
				foreach(var exception in aggException.InnerExceptions)
					exception.Report();
			}, TaskContinuationOptions.OnlyOnFaulted);
		}
    }
}

