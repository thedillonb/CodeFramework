using System;
using SQLite;

namespace CodeFramework.Cache
{
    public class CacheEntry
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Indexed]
        [MaxLength(1024)]
        public string Query { get; set; }

        [MaxLength(1024)]
        public string Path { get; set; }

        public DateTime Updated { get; set; }

        [Ignore]
        public bool IsValid
        {
            get
            {
                return System.IO.File.Exists(Path);
            }
        }

        public T LoadResult<T>() where T : class
        {
            try
            {
                using (var io = System.IO.File.OpenRead(Path))
                {
                    var f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return f.Deserialize(io) as T;
                }
            }
            catch
            {
                return default(T);
            }
        }

        public void SaveResult(object result)
        {
            using (var io = System.IO.File.OpenWrite(Path))
            {
                var f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                f.Serialize(io, result);
                Updated = DateTime.Now;
            }
        }
    }
}

