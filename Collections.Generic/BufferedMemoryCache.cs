namespace Santander.Collections.Generic;

public sealed class BufferedMemoryCache<TKey, TValue> where TKey :
    notnull {
    private readonly object locker = new();

    private SortedDictionary<TKey, TValue> reader;

    private readonly SortedDictionary<TKey, TValue> sortedDictionaryA = new();

    private readonly SortedDictionary<TKey, TValue> sortedDictionaryB = new();

    public event EventHandler? Toggled;

    private SortedDictionary<TKey, TValue> writer;

    public BufferedMemoryCache() {
        reader = sortedDictionaryA;
        writer = sortedDictionaryB;
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
