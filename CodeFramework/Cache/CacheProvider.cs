using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;

namespace CodeFramework.Cache
{
    public class CacheProvider : IEnumerable<CacheEntry>
    {
        private object _lock = new object();
        private SQLiteConnection _cacheDatabase;

        private static string CachePath = System.IO.Path.Combine (MonoTouch.Utilities.BaseDir, "Library/Caches/codeframework.cache/");

        static CacheProvider()
        {
            //Make sure the cachePath exists
            if (!System.IO.Directory.Exists(CachePath))
                System.IO.Directory.CreateDirectory(CachePath);
        }

        public CacheProvider(SQLiteConnection cacheDatabase)
        {
            cacheDatabase.CreateTable<CacheEntry>();
            _cacheDatabase = cacheDatabase;
        }

        public CacheEntry GetEntry(string query)
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

                return cacheEntry;
            }
        }

        public T Get<T>(string query) where T : new()
        {
            var cacheEntry = GetEntry(query);

            try
            {
                return cacheEntry.LoadResult<T>();
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public void Set(string query, object content, string cacheTag = null)
        {
            lock (_lock)
            {
                var queries = _cacheDatabase.Query<CacheEntry>("select * from CacheEntry where query = ?", query);
                var cacheEntry = queries.FirstOrDefault();
                if (cacheEntry != null)
                {
                    cacheEntry.SaveResult(content);
                    cacheEntry.CacheTag = cacheTag;
                    _cacheDatabase.Update(cacheEntry);
                }
                else
                {
                    cacheEntry = new CacheEntry { Query = query, Path = System.IO.Path.Combine(CachePath, System.IO.Path.GetTempFileName()), CacheTag = cacheTag };
                    cacheEntry.SaveResult(content);
                    try
                    {
                        if (_cacheDatabase.Insert(cacheEntry) != 1)
                            throw new InvalidOperationException("Did not insert cache object");
                    }
                    catch
                    {
                        //Clean up the file
                        cacheEntry.Delete();
                    }
                }
            }
        }

        public void DeleteAll()
        {
            lock (_lock)
            {
                foreach (var entry in _cacheDatabase.Table<CacheEntry>())
                    entry.Delete();
                _cacheDatabase.DeleteAll<CacheEntry>();
            }
        }

        public void Delete(string query)
        {
            lock (_lock)
            {
                var cacheEntry = _cacheDatabase.Table<CacheEntry>().Where(x => x.Query.Equals(query)).FirstOrDefault();
                if (cacheEntry != null)
                {
                    cacheEntry.Delete();
                    _cacheDatabase.Delete(cacheEntry);
                }
            }
        }

        public void DeleteWhereStartingWith(string query)
        {
            lock (_lock)
            {
                foreach (var cacheEntry in _cacheDatabase.Table<CacheEntry>().Where(x => x.Query.StartsWith(query)).ToList())
                {
                    cacheEntry.Delete();
                    _cacheDatabase.Delete(cacheEntry);
                }
            }
        }

        IEnumerator<CacheEntry> IEnumerable<CacheEntry>.GetEnumerator()
        {
            return _cacheDatabase.Table<CacheEntry>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _cacheDatabase.Table<CacheEntry>().GetEnumerator();
        }
    }
}

