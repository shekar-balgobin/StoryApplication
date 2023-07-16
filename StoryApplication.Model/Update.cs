namespace Santander.StoryApplication.Model;

public record class Update {
    public required uint[] Items { get; init; }

    public required string[] Profiles { get; init; }
}
