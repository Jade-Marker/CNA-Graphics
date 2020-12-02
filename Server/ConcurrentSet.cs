using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Server
{
    public class ConcurrentSet<T>
    {
        private ConcurrentDictionary<T, T> dictionary = new ConcurrentDictionary<T, T>();

        public IEnumerator<T> GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }

        public void TryAdd(T value)
        {
            dictionary.TryAdd(value, value);
        }

        public void TryRemove(T value)
        {
            dictionary.TryRemove(value, out value);
        }
    }
}
