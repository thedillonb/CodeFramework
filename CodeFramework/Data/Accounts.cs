using System.Collections.Generic;
using MonoTouch;
using System.Collections;
using System.Linq;
using SQLite;

namespace CodeFramework.Data
{
//    public interface IAccounts<T> : IEnumerable<T> where T : Account
//    {
//        /// <summary>
//        /// Insert the specified account.
//        /// </summary>
//        void Insert(T account);
//
//        /// <summary>
//        /// Remove the specified account.
//        /// </summary>
//        void Remove(T account);
//
//        /// <summary>
//        /// Update this instance in the database
//        /// </summary>
//        void Update(T account);
//    }

	/// <summary>
	/// A collection of accounts within the system
	/// </summary>
	public class Accounts<T> : IEnumerable<T> where T : Account, new()
	{
		private SQLiteConnection _userDatabase;
		private static T _activeAccount;

		public T ActiveAccount
		{
			get { return _activeAccount; }
			set { _activeAccount = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeFramework.Data.Accounts`1"/> class.
		/// </summary>
		/// <param name="userDatabase">User database.</param>
		public Accounts(SQLiteConnection userDatabase)
		{
			_userDatabase = userDatabase;
		}

		/// <summary>
		/// Gets the count of accounts in the database
		/// </summary>
		public int Count 
		{
			get { return _userDatabase.Table<T>().Count(); }
		}

		/// <summary>
		/// Gets the default account
		/// </summary>
		public T GetDefault()
		{
			var id = Utilities.Defaults.IntForKey("DEFAULT_ACCOUNT");
			return _userDatabase.Table<T>().SingleOrDefault(x => x.Id == id);
		}

		/// <summary>
		/// Sets the default account
		/// </summary>
		public void SetDefault(T account)
		{
			if (account == null)
				Utilities.Defaults.RemoveObject("DEFAULT_ACCOUNT");
			else
				Utilities.Defaults.SetInt(account.Id, "DEFAULT_ACCOUNT");
			Utilities.Defaults.Synchronize();
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
		public void Insert(T account)
		{
			_userDatabase.Insert(account);
		}

		/// <summary>
		/// Remove the specified account.
		/// </summary>
		public void Remove(T account)
		{
			account.DeleteAccount();
			_userDatabase.Delete(account);
		}

		/// <summary>
		/// Update this instance in the database
		/// </summary>
		public void Update(T account)
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
		public bool Exists(T account)
		{
			return Find(account.Username) != null;
		}

		/// <summary>
		/// Find the specified account via it's username
		/// </summary>
		public T Find(string username)
		{
			var query = _userDatabase.Query<T>("select * from Account where LOWER(Username) = LOWER(?)", username);
			if (query.Count > 0)
				return query[0];
			return null;
		}
	}

}

