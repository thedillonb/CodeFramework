using System.Collections.Generic;
using CodeFramework.Core.Data;

namespace CodeFramework.Core.Services
{
    public interface IAccountsService
    {
        /// <summary>
        /// Gets the accounts directory
        /// </summary>
        string AccountsDir { get; } 

        /// <summary>
        /// Gets the cache directory for the accounts
        /// </summary>
        string CacheDir { get;  }
    }

    public interface IAccountsService<TAccount> : IAccountsService, IEnumerable<TAccount> where TAccount : IAccount
    {
        /// <summary>
        /// Gets the active account
        /// </summary>
        TAccount ActiveAccount { get; }

        /// <summary>
        /// Sets the active account
        /// </summary>
        /// <param name="account"></param>
        void SetActiveAccount(TAccount account);

        /// <summary>
        /// Gets the default account
        /// </summary>
        TAccount GetDefault();

        /// <summary>
        /// Sets the default account
        /// </summary>
        void SetDefault(TAccount account);

        /// <summary>
        /// Insert the specified account.
        /// </summary>
        void Insert(TAccount account);

        /// <summary>
        /// Remove the specified account.
        /// </summary>
        void Remove(TAccount account);

        /// <summary>
        /// Update this instance in the database
        /// </summary>
        void Update(TAccount account);

        /// <summary>
        /// Remove the specified username.
        /// </summary>
        void Remove(string username);

        /// <summary>
        /// Checks to see whether a specific account exists (Username comparison)
        /// </summary>
        bool Exists(TAccount account);

        /// <summary>
        /// Find the specified account via it's username
        /// </summary>
        TAccount Find(string username);

        /// <summary>
        /// Find the specified account via it's username
        /// </summary>
        TAccount Find(int id);
    }
}