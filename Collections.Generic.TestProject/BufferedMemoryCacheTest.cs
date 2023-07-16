using FluentAssertions;

namespace Santander.Collections.Generic.TestProject;

public sealed class BufferedMemoryCacheTest {
    [Fact]
    public void Toggle() {
        var bufferedMemoryCache = new BufferedMemoryCache<object, object>();
        var reader = bufferedMemoryCache.Reader as IEnumerable<KeyValuePair<object, object>>;
        var writer = bufferedMemoryCache.Writer as IEnumerable<KeyValuePair<object, object>>;

        bufferedMemoryCache.Toggle();

        reader.Should().NotBeSameAs(bufferedMemoryCache.Reader);
        writer.Should().NotBeSameAs(bufferedMemoryCache.Writer);

        reader.Should().BeSameAs(bufferedMemoryCache.Writer);
        writer.Should().BeSameAs(bufferedMemoryCache.Reader);

        bufferedMemoryCache.Toggle();

        reader.Should().NotBeSameAs(bufferedMemoryCache.Writer);
        writer.Should().NotBeSameAs(bufferedMemoryCache.Reader);

        reader.Should().BeSameAs(bufferedMemoryCache.Reader);
        writer.Should().BeSameAs(bufferedMemoryCache.Writer);
    }
}
