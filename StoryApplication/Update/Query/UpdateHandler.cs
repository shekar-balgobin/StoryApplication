using MediatR;
using System.Net.Http.Json;

namespace Santander.StoryApplication.Update.Query;

public sealed class UpdateHandler :
    IRequestHandler<GetItemByIdentifierQuery, Model.Item?>,
    IRequestHandler<GetUpdateQuery, Model.Update?> {
    private readonly IHttpClientFactory httpClientFactory;

    public UpdateHandler(IHttpClientFactory httpClientFactory) =>
        this.httpClientFactory = httpClientFactory;

    public async Task<Model.Item?> Handle(GetItemByIdentifierQuery request, CancellationToken cancellationToken) =>
    await httpClientFactory
        .CreateClient(name: nameof(UpdateHandler))
        .GetFromJsonAsync<Model.Story>(requestUri: $"v0/item/{request.Id}.json", cancellationToken)
        .ConfigureAwait(continueOnCapturedContext: false);

    public async Task<Model.Update?> Handle(GetUpdateQuery request, CancellationToken cancellationToken) =>
        await httpClientFactory
            .CreateClient(name: nameof(UpdateHandler))
            .GetFromJsonAsync<Model.Update>(requestUri: "v0/updates.json", cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
}
