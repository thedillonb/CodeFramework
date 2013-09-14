using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;

namespace CodeFramework.Cache
{
    public class CacheProvider
    {
        private object _lock = new object();
        private SQLiteConnection _cacheDatabase;

        public CacheProvider(SQLiteConnection cacheDatabase)
        {
            _cacheDatabase = cacheDatabase;
        }

        public T Get<T>(string query) where T : class
        {
            lock (_lock)
            {
                var queries = _cacheDatabase.Query<CacheEntry>("select * from CacheEntry where query = ?", query);
                var cacheEntry = queries.FirstOrDefault();

                if (cacheEntry == null)
                    return null;

                if (!cacheEntry.IsValid)
                {
                    //Remove it since it's not valid anymore
                    _cacheDatabase.Delete(cacheEntry);
                    return null;
                }

                try
                {
                    return cacheEntry.LoadResult<T>();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public void Set(string query, object content)
        {
            lock (_lock)
            {
                var queries = _cacheDatabase.Query<CacheEntry>("select * from CacheEntry where query = ?", query);
                var cacheEntry = queries.FirstOrDefault();
                if (cacheEntry != null)
                {
                    cacheEntry.SaveResult(content);
                    _cacheDatabase.Update(cacheEntry);
                }
                else
                {
                    cacheEntry = new CacheEntry { Query = query, Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempFileName()) };
                    cacheEntry.SaveResult(content);
                    try
                    {
                        if (_cacheDatabase.Insert(cacheEntry) != 1)
                            throw new InvalidOperationException("Did not insert cache object");
                    }
                    catch
                    {
                        //Clean up the file
                        try { System.IO.File.Delete(cacheEntry.Path); } catch { }
                    }
                }
            }
        }

        public void Delete(string query)
        {
            lock (_lock)
            {
                var cacheEntry = _cacheDatabase.Table<CacheEntry>().Where(x => x.Query.Equals(query)).FirstOrDefault();
                if (cacheEntry != null)
                    _cacheDatabase.Delete(cacheEntry);
            }
        }

        public void DeleteWhereStartingWith(string query)
        {
            lock (_lock)
            {
                foreach (var e in _cacheDatabase.Table<CacheEntry>().Where(x => x.Query.StartsWith(query)))
                    _cacheDatabase.Delete(e);
            }
        }
    }
}

