using SQLite;

namespace CodeFramework.Core.Cache
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

        [MaxLength(256)]
        public string CacheTag { get; set; }

        [Ignore]
        public bool IsValid
        {
            get
            {
                return System.IO.File.Exists(Path);
            }
        }

        public bool Delete()
        {
            try
            {
                if (!IsValid) return false;
                System.IO.File.Delete(Path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T LoadResult<T>() where T : new()
        {
            try
            {
				var io = System.IO.File.ReadAllText(Path);
				var jsonSerializer = Cirrious.CrossCore.Mvx.Resolve<CodeFramework.Core.Services.IJsonSerializationService>();
				return jsonSerializer.Deserialize<T>(io);
            }
            catch
            {
                return default(T);
            }
        }

        public void SaveResult(object data)
        {
			var jsonSerializer = Cirrious.CrossCore.Mvx.Resolve<CodeFramework.Core.Services.IJsonSerializationService>();
			var a = jsonSerializer.Serialize(data);
			System.IO.File.WriteAllText(Path, a);
        }
    }
}

