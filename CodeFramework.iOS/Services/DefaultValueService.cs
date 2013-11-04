using System;
using CodeFramework.Core.Services;
using MonoTouch;

namespace CodeFramework.iOS.Services
{
    public class DefaultValueService : IDefaultValueService
    {
        public DefaultValueService()
        {
            Console.WriteLine("COOL!");
        }

        public T Get<T>(string key)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)Utilities.Defaults.IntForKey(key);
            throw new Exception("Key does not exist in Default database.");
        }

        public bool TryGet<T>(string key, out T value)
        {
            var val = Utilities.Defaults.DataForKey(key);
            if (val == null)
            {
                value = default(T);
                return false;
            }
            value = Get<T>(key);
            return true;
        }

        public void Set(string key, object value)
        {
            if (value == null)
                Utilities.Defaults.RemoveObject(key);
            else if (value is int)
                Utilities.Defaults.SetInt((int) value, key);
            Utilities.Defaults.Synchronize();
        }
    }
}
