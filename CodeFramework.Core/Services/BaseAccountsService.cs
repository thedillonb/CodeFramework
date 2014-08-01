using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Akavache;
using CodeFramework.Core.Data;
using Xamarin.Utilities.Core.Services;

namespace CodeFramework.Core.Services
{
    public abstract class BaseAccountsService<TAccount> : IAccountsService where TAccount : IAccount, new()
    {
        private readonly Subject<IAccount> _accountSubject = new Subject<IAccount>();
        private readonly IDefaultValueService _defaults;
        private IAccount _activeAccount;

        public IAccount ActiveAccount
        {
            get { return _activeAccount; }
            set
            {
                if (value != null && !(value is TAccount))
                    throw new InvalidOperationException("Set Active Account object is not of correct type");
                _defaults.Set("DEFAULT_ACCOUNT", value == null ? null : value.Key);
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
        }

        public IAccount GetDefault()
        {
            string id;
            return !_defaults.TryGet("DEFAULT_ACCOUNT", out id) ? null : Find(id);
        }

        public void Insert(IAccount account)
        {
            BlobCache.UserAccount.InsertObject("user_" + account.Key, account).Wait();
        }

        public void Remove(IAccount account)
        {
            BlobCache.UserAccount.Invalidate("user_" + account.Key).Wait();
        }

        public void Update(IAccount account)
        {
            Insert(account);
        }

        public bool Exists(IAccount account)
        {
            return Find(account.Domain, account.Username) != null;
        }

        public IAccount Find(string domain, string username)
        {
            return Find("user_" + domain + username);
        }

        public IAccount Find(string key)
        {
            return BlobCache.UserAccount.GetObjectAsync<TAccount>("user_" + key).Wait();
        }

        public IEnumerator<IAccount> GetEnumerator()
        {
            return BlobCache.UserAccount.GetAllKeys()
                .Where(x => x.StartsWith("user_"))
                .Select(k => BlobCache.UserAccount.GetObjectAsync<TAccount>(k).Wait())
                .Select(dummy => (IAccount) dummy).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
