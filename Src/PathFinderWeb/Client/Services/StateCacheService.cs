using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PathFinderWeb.Client.Services
{
    public class StateCacheService : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly ConcurrentDictionary<string, object> _apiCache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public StateCacheService()
        {
        }

        public void Clear() => _apiCache.Clear();

        public int Count => _apiCache.Count;

        public T GetOrCreate<T>(Func<T> createNew) => (T)_apiCache.GetOrAdd(typeof(T).FullName, k => createNew()!);

        public T GetOrCreate<T>(string id, Func<T> createNew) => (T)_apiCache.GetOrAdd(id, k => createNew()!);

        public void Set<T>(T value) => _apiCache[typeof(T).FullName] = value!;

        public void Set<T>(string id, T value) => _apiCache[id] = value!;

        public bool TryGetValue<T>(out T value) => TryGetValue<T>(typeof(T).FullName, out value);

        public bool TryGetValue<T>(string id, out T value)
        {
            value = default!;
            if (!_apiCache.TryGetValue(id, out object cacheValue)) return false;

            value = (T)cacheValue;
            return true;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _apiCache.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _apiCache.GetEnumerator();
    }
}