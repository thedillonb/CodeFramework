﻿using CodeFramework.Core.Services;
using MonoTouch;

namespace CodeFramework.iOS.Services
{
    public class AccountPreferencesService : IAccountPreferencesService
    {
        public string AccountsDir
        {
            get { return System.IO.Path.Combine(Utilities.BaseDir, "Documents/accounts"); }
        }
    }
}
