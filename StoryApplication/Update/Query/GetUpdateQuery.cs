using MediatR;

namespace Santander.StoryApplication.Update.Query;

public sealed record class GetUpdateQuery :
    IRequest<Model.Update?> {
}
