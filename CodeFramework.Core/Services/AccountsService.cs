using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CodeFramework.Core.Data;
using SQLite;

namespace CodeFramework.Core.Services
{
    public abstract class AccountsService<TAccount> : IAccountsService<TAccount> where TAccount : IAccount, new()
    {
        private readonly string _accountsPath;
        private readonly SQLiteConnection _userDatabase;

        public TAccount ActiveAccount { get; private set; }

        public abstract string AccountsDir { get; }

        public abstract string CacheDir { get; }

        protected AccountsService(string accountsPath)
        {
            _accountsPath = accountsPath;

            // Assure creation of the accounts path
            if (!Directory.Exists(accountsPath))
                Directory.CreateDirectory(accountsPath);

            _userDatabase = new SQLiteConnection(Path.Combine(accountsPath, "accounts.db"));
            _userDatabase.CreateTable<TAccount>();
        }

        protected abstract TAccount OnSetActiveAccount(TAccount account);

        public abstract TAccount GetDefault();

        public abstract void SetDefault(TAccount account);

        public void SetActiveAccount(TAccount account)
        {
            var accountDir = CreateAccountDirectory(account);
            if (!Directory.Exists(accountDir))
                Directory.CreateDirectory(accountDir);

            ActiveAccount = OnSetActiveAccount(account);
        }

        protected string CreateAccountDirectory(TAccount account)
        {
            return Path.Combine(_accountsPath, account.Id.ToString(CultureInfo.InvariantCulture));
        }

        public void Insert(TAccount account)
        {
            _userDatabase.Insert(account);
        }

        public void Remove(TAccount account)
        {
            _userDatabase.Delete(account);
            var accountDir = CreateAccountDirectory(account);

            if (!Directory.Exists(accountDir))
                return;
            Directory.Delete(accountDir, true);
        }

        public void Update(TAccount account)
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

        public bool Exists(TAccount account)
        {
            return Find(account.Username) != null;
        }

        public TAccount Find(string username)
        {
            var lowerUser = username.ToLower();
            return _userDatabase.Find<TAccount>(x => x.Username.ToLower().Equals(lowerUser));
        }

        public TAccount Find(int id)
        {
            var query = _userDatabase.Find<TAccount>(x => x.Id == id);
            return query;
        }

        public IEnumerable<TAccount> GetAccounts()
        {
            return _userDatabase.Table<TAccount>();
        }
    }
}
