using FluentAssertions;

namespace Santander.Collections.Generic.TestProject;

public sealed class BufferedMemoryCacheTest {
    [Fact]
    public void Reader() {
        var bufferedMemoryCache = new BufferedMemoryCache<bool, bool>();
        var actual = bufferedMemoryCache.Reader;

        actual.Should().BeAssignableTo<IReadOnlyDictionary<bool, bool>>();
    }

    [Fact]
    public void Writer() {
        var bufferedMemoryCache = new BufferedMemoryCache<bool, bool>();
        var actual = bufferedMemoryCache.Writer;

        actual.Should().BeAssignableTo<IDictionary<bool, bool>>();
    }
}
