namespace Santander.StoryApplication.Model;

public record class Item {
    public string? By { get; init; }

    public bool? Dead { get; init; }

    public bool? Deleted { get; init; }

    public uint? Descendants { get; init; }

    public required uint Id { get; init; }

    public uint[]? Kids { get; init; }

    public uint? Parent { get; init; }

    public uint[]? Parts { get; init; }

    public uint? Poll { get; init; }

    public required uint Score { get; init; }

    public string? Text { get; init; }

    public uint? Time { get; init; }

    public string? Title { get; init; }

    public required string Type { get; init; }

    public Uri? Url { get; init; }
}
