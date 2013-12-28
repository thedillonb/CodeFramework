using System.Runtime.Serialization;
using SQLite;

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
                using (var stream = new System.IO.MemoryStream())
                {
                    var buffer = System.Text.Encoding.UTF8.GetBytes(RawData);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Position = 0;
                    var d = new DataContractSerializer(typeof(T));
                    return (T)d.ReadObject(stream);
                }
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
            using (var stream = new System.IO.MemoryStream())
            {
                var d = new DataContractSerializer(o.GetType());
                d.WriteObject(stream, o);
                stream.Position = 0;
                RawData = new System.IO.StreamReader(stream).ReadToEnd();
            }
        }
    }
}

