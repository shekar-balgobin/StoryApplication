using MediatR;

namespace Santander.StoryApplication.Update.Query;

public sealed record class GetItemByIdentifierQuery :
    IRequest<Model.Item> {
    public required uint Id { get; init; }
}
