using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reactive.Subjects;
using CodeFramework.Core.Data;
using SQLite;
using Xamarin.Utilities.Core.Services;

namespace CodeFramework.Core.Services
{
    public abstract class BaseAccountsService<TAccount> : IAccountsService where TAccount : IAccount, new()
    {
        private readonly Subject<IAccount> _accountSubject = new Subject<IAccount>();
        private readonly SQLiteConnection _userDatabase;
        private readonly IDefaultValueService _defaults;
        private readonly string _accountsPath;
        private IAccount _activeAccount;

        public IAccount ActiveAccount
        {
            get { return _activeAccount; }
            set
            {
                if (value != null && !(value is TAccount))
                    throw new InvalidOperationException("Set Active Account object is not of correct type");

                if (value != null)
                {
                    var accountDir = CreateAccountDirectory(value);
                    if (!Directory.Exists(accountDir))
                        Directory.CreateDirectory(accountDir);
                }

                if (value == null)
                    _defaults.Set("DEFAULT_ACCOUNT", null);
                else
                    _defaults.Set("DEFAULT_ACCOUNT", value.Id);

                _activeAccount = value;

                _accountSubject.OnNext(value);
            }
        }

        public IObservable<IAccount> ActiveAccountChanged
        {
            get { return _accountSubject; }
        }

        protected BaseAccountsService(IDefaultValueService defaults)
        {
            _defaults = defaults;
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..");
            _accountsPath = Path.Combine(basePath, "Documents/accounts");

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
            
        protected string CreateAccountDirectory(IAccount account)
        {
            return Path.Combine(_accountsPath, account.Id.ToString(CultureInfo.InvariantCulture));
        }

        public void Insert(IAccount account)
        {
			lock (_userDatabase)
			{
				_userDatabase.Insert(account);
			}
        }

        public void Remove(IAccount account)
        {
			lock (_userDatabase)
			{
				_userDatabase.Delete(account);
			}
            var accountDir = CreateAccountDirectory(account);

            if (!Directory.Exists(accountDir))
                return;
            Directory.Delete(accountDir, true);
        }

        public void Update(IAccount account)
        {
			lock (_userDatabase)
			{
				_userDatabase.Update(account);
			}
        }

        public bool Exists(IAccount account)
        {
			return Find(account.Id) != null;
        }

        public IAccount Find(int id)
        {
			lock (_userDatabase)
			{
                var query = _userDatabase.Find<TAccount>(x => x.Id == id);
				return query;
			}
        }


        public IEnumerator<IAccount> GetEnumerator()
        {
            return (IEnumerator<IAccount>)_userDatabase.Table<TAccount>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
