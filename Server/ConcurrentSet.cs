using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Server
{
    public class ConcurrentSet<T>
    {
        private ConcurrentDictionary<T, T> _dictionary = new ConcurrentDictionary<T, T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        public void TryAdd(T value)
        {
            _dictionary.TryAdd(value, value);
        }

        public void TryRemove(T value)
        {
            _dictionary.TryRemove(value, out value);
        }
    }
}
