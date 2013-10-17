using System.Collections.Generic;
using System.Collections;
using System.Linq;
using SQLite;
using MonoTouch;

namespace CodeFramework.Data
{
    public static class Accounts
    {
        public static IAccounts<Account> Instance;
    }

    public interface IAccounts<out T> : IEnumerable<T> where T : Account, new()
    {
        int Count { get; }
        T ActiveAccount { get; }
        void Insert(Account account);
        void Remove(Account account);
        void Update(Account account);
        T Find(string username);
        T Find(int id);
        T GetDefault();
        void SetDefault(Account account);
        void SetActiveAccount(Account account);
    }

    /// <summary>
    /// A collection of accounts within the system
    /// </summary>
    public class Accounts<T> : IAccounts<T> where T : Account, new()
    {
        private static readonly string AccountsPath = System.IO.Path.Combine(Utilities.BaseDir, "Documents/accounts");
        private SQLiteConnection _userDatabase;

        public T ActiveAccount { get; private set; }

        public void SetActiveAccount(Account account)
        {
            ActiveAccount = account as T;

            //Initialize the active account so it loads the settings and what not
            if (ActiveAccount != null)
                ActiveAccount.Initialize();
        }

        /// <summary>
        /// Gets the default account
        /// </summary>
        public T GetDefault()
        {
            var id = Utilities.Defaults.IntForKey("DEFAULT_ACCOUNT");
            return Find(id);
        }

        /// <summary>
        /// Sets the default account
        /// </summary>
        public void SetDefault(Account account)
        {
            if (account == null)
                Utilities.Defaults.RemoveObject("DEFAULT_ACCOUNT");
            else
                Utilities.Defaults.SetInt(account.Id, "DEFAULT_ACCOUNT");
            Utilities.Defaults.Synchronize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFramework.Data.Accounts`1"/> class.
        /// </summary>
        /// <param name="userDatabase">User database.</param>
        public Accounts()
        {
            if (!System.IO.Directory.Exists(AccountsPath))
                System.IO.Directory.CreateDirectory(AccountsPath);
            _userDatabase = new SQLiteConnection(System.IO.Path.Combine(AccountsPath, "accounts.db"));
            _userDatabase.CreateTable<T>();
        }

        /// <summary>
        /// Gets the count of accounts in the database
        /// </summary>
        public int Count 
        {
            get { return _userDatabase.Table<T>().Count(); }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        public IEnumerator<T> GetEnumerator ()
        {
            return _userDatabase.Table<T>().GetEnumerator();
        }

        /// <summary>
        /// Insert the specified account.
        /// </summary>
        public void Insert(Account account)
        {
            _userDatabase.Insert(account);
        }

        /// <summary>
        /// Remove the specified account.
        /// </summary>
        public void Remove(Account account)
        {
            _userDatabase.Delete(account);
            account.Destory();
        }

        /// <summary>
        /// Update this instance in the database
        /// </summary>
        public void Update(Account account)
        {
            _userDatabase.Update(account);
        }

        /// <summary>
        /// Remove the specified username.
        /// </summary>
        public void Remove(string username)
        {
            var q = from f in _userDatabase.Table<T>()
                where f.Username == username
                    select f;
            var account = q.FirstOrDefault();
            if (account != null)
                Remove(account);
        }

        /// <summary>
        /// Checks to see whether a specific account exists (Username comparison)
        /// </summary>
        public bool Exists(Account account)
        {
            return Find(account.Username) != null;
        }

        /// <summary>
        /// Find the specified account via it's username
        /// </summary>
        public T Find(string username)
        {
            var lowerUser = username.ToLower();
            return _userDatabase.Find<T>(x => x.Username.ToLower().Equals(lowerUser));
        }

        /// <summary>
        /// Find the specified account via it's username
        /// </summary>
        public T Find(int id)
        {
            var query = _userDatabase.Find<T>(x => x.Id == id);
            return query;
        }
    }

}

