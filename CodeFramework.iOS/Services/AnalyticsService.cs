using System;
using CodeFramework.Core.Services;
using Parse;
using Newtonsoft;

namespace CodeFramework.iOS.Services
{
	public class AnalyticsService : IAnalyticsService
    {
        private bool _enabled;

        public void Init(string id, string key)
		{
            ParseClient.Initialize(id, key);
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => LogException(e.ExceptionObject as Exception, true);

            if (MonoTouch.Utilities.Defaults.ValueForKey(new MonoTouch.Foundation.NSString("CodeFramework.Analytics")) == null)
            {
                Enabled = true;
            }
            else
                _enabled = MonoTouch.Utilities.Defaults.BoolForKey("CodeFramework.Analytics");

            try
            {
                ReportExceptionInLog();
            }
            catch
            {
            }
		}

		public bool Enabled
		{
			get
            {
                return _enabled;
            }
			set
            {
                _enabled = value;
                MonoTouch.Utilities.Defaults.SetBool(_enabled, "CodeFramework.Analytics");
                MonoTouch.Utilities.Defaults.Synchronize();
            }
		}

		public void LogException(Exception e)
		{
			LogException(e, false);
		}

		public void LogException(Exception e, bool fatal)
		{
			if (e == null)
				return;

            if (!fatal)
            {
                var errObject = new ParseObject("Error");
                errObject["Date"] = DateTime.Now;
                errObject["Message"] = e.Message;
                errObject["StackTrace"] = e.StackTrace;
                errObject["TargetSite"] = e.TargetSite.ToString();
                errObject["Fatal"] = 0;
                if (e.InnerException != null)
                    errObject["Inner"] = e.InnerException.Message;
                errObject.SaveAsync();
            }
            else
            {
                SaveExceptionInLog(e);
            }
		}

        private void SaveExceptionInLog(Exception e)
        {
            var err = new SerializedError
            {
                Date = DateTime.Now,
                Message = e.Message,
                StackTrace = e.StackTrace,
                TargetSite = e.TargetSite.ToString(),
                Fatal = true,
            };

            if (e.InnerException != null)
                err.InnerException = e.InnerException.Message;

            var fileData = Newtonsoft.Json.JsonConvert.SerializeObject(err);
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "crash_report.json");
            System.IO.File.WriteAllText(path, fileData, System.Text.Encoding.UTF8);
        }

        private void ReportExceptionInLog()
        {
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "crash_report.json");
            if (!System.IO.File.Exists(path))
                return;

            var fileData = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
            System.IO.File.Delete(path);
            var err = Newtonsoft.Json.JsonConvert.DeserializeObject<SerializedError>(fileData);
            if (err == null)
                return;

            var errObject = new ParseObject("Error");
            errObject["Date"] = err.Date;
            errObject["Message"] = err.Message;
            errObject["StackTrace"] = err.StackTrace;
            errObject["TargetSite"] = err.TargetSite;
            errObject["Fatal"] = err.Fatal ? 1 : 0;
            errObject["Inner"] = err.InnerException;
            errObject.SaveAsync();
        }

        private class SerializedError
        {
            public DateTime Date { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public string TargetSite { get; set; }
            public bool Fatal { get; set; }
            public string InnerException { get; set; }
        }
    }
}

