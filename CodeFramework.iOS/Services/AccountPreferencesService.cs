using System;
using System.IO;
using CodeFramework.Core.Services;

namespace CodeFramework.iOS.Services
{
    public class AccountPreferencesService : IAccountPreferencesService
    {
        public readonly static string BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..");

        public string AccountsDir
        {
            get { return Path.Combine(BaseDir, "Documents/accounts"); }
        }
    }
}
