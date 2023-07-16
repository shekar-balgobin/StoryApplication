using MediatR;

namespace Santander.StoryApplication.Story.Query;

public sealed record class GetStoryByIdentifierQuery :
    IRequest<Model.Story?> {
    public required uint Id { get; init; }
}
