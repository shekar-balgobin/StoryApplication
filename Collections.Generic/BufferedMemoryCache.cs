using System.Collections.Concurrent;

namespace Santander.Collections.Generic;

public sealed class BufferedMemoryCache<TKey, TValue> where TKey :
    notnull {
    private readonly object locker = new();

    private ConcurrentDictionary<TKey, TValue> reader;

    private readonly ConcurrentDictionary<TKey, TValue> concurrentDictionaryA = new();

    private readonly ConcurrentDictionary<TKey, TValue> concurrentDictionaryB = new();

    public event EventHandler? Toggled;

    private ConcurrentDictionary<TKey, TValue> writer;

    public BufferedMemoryCache() {
        reader = concurrentDictionaryA;
        writer = concurrentDictionaryB;
    }

    public IReadOnlyDictionary<TKey, TValue> Reader => reader;

    public void Toggle() {
        lock (locker) {
            (reader, writer) = (writer, reader);
            writer.Clear();
        }

        Toggled?.Invoke(sender: this, EventArgs.Empty);
    }

    public IDictionary<TKey, TValue> Writer => writer;
}
