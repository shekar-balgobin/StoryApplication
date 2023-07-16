using MediatR;

namespace Santander.StoryApplication.Story.Query;

public sealed record class GetStoryIdentifierStreamQuery :
    IStreamRequest<uint> {
}
