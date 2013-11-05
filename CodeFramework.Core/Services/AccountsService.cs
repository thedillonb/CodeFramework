using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CodeFramework.Core.Data;
using SQLite;

namespace CodeFramework.Core.Services
{
    public abstract class AccountsService<TAccount> : IAccountsService where TAccount : class, IAccount, new()
    {
        private readonly SQLiteConnection _userDatabase;
        private readonly IDefaultValueService _defaults;
        private readonly string _accountsPath;

        public IAccount ActiveAccount { get; private set; }

        protected AccountsService(IDefaultValueService defaults, IAccountPreferencesService accountPreferences)
        {
            _defaults = defaults;
            _accountsPath = accountPreferences.AccountsDir;

            // Assure creation of the accounts path
            if (!Directory.Exists(_accountsPath))
                Directory.CreateDirectory(_accountsPath);

            _userDatabase = new SQLiteConnection(Path.Combine(_accountsPath, "accounts.db"));
            _userDatabase.CreateTable<TAccount>();
        }

        public IAccount GetDefault()
        {
            int id;
            return !_defaults.TryGet("DEFAULT_ACCOUNT", out id) ? null : Find(id);
        }

        public void SetDefault(IAccount account)
        {
            if (account == null)
                _defaults.Set("DEFAULT_ACCOUNT", null);
            else
                _defaults.Set("DEFAULT_ACCOUNT", account.Id);
        }

        public void SetActiveAccount(IAccount account)
        {
            var accountDir = CreateAccountDirectory(account);
            if (!Directory.Exists(accountDir))
                Directory.CreateDirectory(accountDir);

            ActiveAccount = account;
        }

        protected string CreateAccountDirectory(IAccount account)
        {
            return Path.Combine(_accountsPath, account.Id.ToString(CultureInfo.InvariantCulture));
        }

        public void Insert(IAccount account)
        {
            _userDatabase.Insert(account);
        }

        public void Remove(IAccount account)
        {
            _userDatabase.Delete(account);
            var accountDir = CreateAccountDirectory(account);

            if (!Directory.Exists(accountDir))
                return;
            Directory.Delete(accountDir, true);
        }

        public void Update(IAccount account)
        {
            _userDatabase.Update(account);
        }

        public void Remove(string username)
        {
            var q = from f in _userDatabase.Table<TAccount>()
                    where f.Username == username
                    select f;
            var account = q.FirstOrDefault();
            if (account != null)
                Remove(account);
        }

        public bool Exists(IAccount account)
        {
            return Find(account.Username) != null;
        }

        public IAccount Find(string username)
        {
            var lowerUser = username.ToLower();
            return _userDatabase.Find<TAccount>(x => x.Username.ToLower().Equals(lowerUser));
        }

        public IAccount Find(int id)
        {
            var query = _userDatabase.Find<TAccount>(x => x.Id == id);
            return query;
        }

        public IEnumerator<IAccount> GetEnumerator()
        {
            return _userDatabase.Table<TAccount>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
