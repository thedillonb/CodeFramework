using System;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.Filters.Models;
using MonoTouch;
using System.IO;
using SQLite;

namespace CodeFramework.Data
{
    public class Account
    {
        private static string AccountsDirectory = Path.Combine(Utilities.BaseDir, "Documents/accounts");
        private SQLiteConnection _database;
        private Cache.CacheProvider _cache;

        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        /// <value>The avatar URL.</value>
        public string AvatarUrl { get; set; }

        [Ignore]
        public SQLiteConnection Database 
        {
            get
            {
                if (_database == null)
                {
                    if (!Directory.Exists(AccountDirectory))
                        Directory.CreateDirectory(AccountDirectory);

                    var dbPath = Path.Combine(AccountDirectory, "settings.db");
                    _database = new SQLiteConnection(dbPath);
                    _database.CreateTable<PinnedRepository>();
                    _database.CreateTable<Filter>();
                    return _database;
                }

                return _database;
            }
        }

        [Ignore]
        public Cache.CacheProvider Cache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new CodeFramework.Cache.CacheProvider(Database);
                }
             
                return _cache;
            }
        }

        [Ignore]
        public string AccountDirectory
        {
            get { return Path.Combine(AccountsDirectory, Id.ToString()); }
        }

        public Account()
        {
        }

        /// <summary>
        /// This creates this account's directory
        /// </summary>
        public void Initialize()
        {
            if (!Directory.Exists(AccountDirectory))
                Directory.CreateDirectory(AccountDirectory);
        }

        /// <summary>
        /// This destorys this account's directory
        /// </summary>
        public void Destory()
        {
            if (!Directory.Exists(AccountDirectory))
                return;
            Cache.DeleteAll();
            Directory.Delete(AccountDirectory, true);
        }
        
        /// <summary>
        /// Gets the pinned resources.
        /// </summary>
        /// <returns>The pinned resources.</returns>
        /// <param name="c">C.</param>
        public List<PinnedRepository> GetPinnedRepositories()
        {
            return Database.Table<PinnedRepository>().OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Adds the pinned repository.
        /// </summary>
        /// <param name="owner">Owner.</param>
        /// <param name="slug">Slug.</param>
        /// <param name="name">Name.</param>
        /// <param name="imageUri">Image URI.</param>
        public void AddPinnedRepository(string owner, string slug, string name, string imageUri)
        {
            var resource = new PinnedRepository { Owner = owner, Slug = slug, Name = name, ImageUri = imageUri };
            Database.Insert(resource);
        }

        /// <summary>
        /// Removes the pinned repository.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void RemovePinnedRepository(int id)
        {
            Database.Delete(new PinnedRepository { Id = id });
        }

        /// <summary>
        /// Gets the pinned repository.
        /// </summary>
        /// <returns>The pinned repository.</returns>
        /// <param name="owner">Owner.</param>
        /// <param name="slug">Slug.</param>
        public PinnedRepository GetPinnedRepository(string owner, string slug)
        {
            return Database.Find<PinnedRepository>(x => x.Owner == owner && x.Slug == slug);
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <returns>The filters.</returns>
        public List<Filter> GetFilters()
        {
            return Database.Table<Filter>().ToList();
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <returns>The filter.</returns>
        /// <param name="key">Key.</param>
        public F GetFilter<F>(string key) where F : FilterModel<F>, new()
        {
            var filter = Database.Find<Filter>(x => x.Type == key);
            if (filter == null)
                return new F();
            var filterModel = filter.GetData<F>();
            return filterModel == null ? new F() : filterModel;
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <returns>The filter.</returns>
        /// <param name="key">Key.</param>
        public F GetFilter<F>(object key) where F : FilterModel<F>, new()
        {
            return GetFilter<F>(key.GetType().Name);
        }

        /// <summary>
        /// Returns a filter object if it exists, null if otherwise
        /// </summary>
        /// <returns>The exist.</returns>
        /// <param name="key">Key.</param>
        private Filter DoesFilterExist(string key)
        {
            return Database.Find<Filter>(x => x.Type == key);
        }

        /// <summary>
        /// Adds the filter
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public void AddFilter<F>(string key, F data)
        {
            RemoveFilters(key);
            var filter = new Filter { Type = key };
            filter.SetData(data);
            Database.Insert(filter);
        }

        /// <summary>
        /// Adds a filter using any object's type as a key
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public void AddFilter<F>(object key, F data)
        {
            AddFilter<F>(key.GetType().Name, data);
        }

        /// <summary>
        /// Removes the filter
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void RemoveFilter(int id)
        {
            Database.Delete(new Filter { Id = id });
        }

        /// <summary>
        /// Removes all filters with a specific key
        /// </summary>
        /// <param name="key">Key.</param>
        public void RemoveFilters(string key)
        {
            var filters = Database.Table<Filter>().Where(x => x.Type == key).ToList();
            foreach (var filter in filters)
                Database.Delete(filter);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Account"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Account"/>.</returns>
        public override string ToString()
        {
            return Username;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Account"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Account"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Account"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            var act = obj as Account;
            return act != null && this.Id.Equals(act.Id);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Gistacular.Data.Account"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}

