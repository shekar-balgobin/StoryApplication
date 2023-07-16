namespace Santander.StoryApplication.ViewModel;

public sealed record class Story {
    public int? CommentCount { get; init; }

    public string? PostedBy { get; init; }

    public required int Score { get; init; }

    public DateTime? Time { get; init; }

    public string? Title { get; init; }

    public Uri? Uri { get; init; }
}
