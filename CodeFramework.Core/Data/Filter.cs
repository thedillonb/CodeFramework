using SQLite;
using Xamarin.Utilities.Core.Services;

namespace CodeFramework.Core.Data
{
    public class Filter
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Type { get; set; }

        [MaxLength(2048)]
        public string RawData { get; set; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T GetData<T>() where T : new()
        {
            try
            {
                var serializer = IoC.Resolve<IJsonSerializationService>();
                return serializer.Deserialize<T>(RawData);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="o">O.</param>
        public void SetData(object o)
        {
            var serializer = IoC.Resolve<IJsonSerializationService>();
            RawData = serializer.Serialize(o);
        }
    }
}

