using System;
using System.Runtime.Serialization;
using Cirrious.CrossCore;
using CodeFramework.Core.Services;
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
            catch (Exception e)
            {
                Mvx.Resolve<IErrorReporter>().ReportError("Unable to deserialize filter", e);
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
                using (var sr = new System.IO.StreamReader(stream))
                {
                    RawData = sr.ReadToEnd();
                }
            }
        }
    }
}
