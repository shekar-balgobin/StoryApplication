using System.Collections.Concurrent;

namespace Santander.Collections.Generic;

public sealed class ConcurrentMemoryCache<TKey, TValue> where TKey :
    notnull {
    private readonly ConcurrentDictionary<TKey, TValue> concurrentDictionary = new();

    public IReadOnlyDictionary<TKey, TValue> Reader => concurrentDictionary;

    public IDictionary<TKey, TValue> Writer => concurrentDictionary;
}
