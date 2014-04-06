using System;
using CodeFramework.Core.Services;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CodeFramework.iOS.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        public string OSVersion
        {
            get
            {
                var v = MonoTouch.Utilities.iOSVersion;
                return String.Format("{0}.{1}", v.Item1, v.Item2);
            }
        }

        public string ApplicationVersion
        {
            get
            {
                return string.Format("{0}.{1}", NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"], NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString());
            }
        }

        public string DeviceName
        {
            get
            {
                return UIDevice.CurrentDevice.Name;
            }
        }
    }
}

