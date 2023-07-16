using MediatR;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace Santander.StoryApplication.Story.Query;

public sealed class StoryHandler :
    IRequestHandler<GetStoryByIdentifierQuery, Model.Story?>,
    IStreamRequestHandler<GetStoryIdentifierStreamQuery, uint> {
    private readonly IHttpClientFactory httpClientFactory;

    public StoryHandler(IHttpClientFactory httpClientFactory) =>
        this.httpClientFactory = httpClientFactory;

    public async Task<Model.Story?> Handle(GetStoryByIdentifierQuery request, CancellationToken cancellationToken) =>
        await httpClientFactory
            .CreateClient(name: nameof(StoryHandler))
            .GetFromJsonAsync<Model.Story>(requestUri: $"v0/item/{request.Id}.json", cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

    public async IAsyncEnumerable<uint> Handle(GetStoryIdentifierStreamQuery request, [EnumeratorCancellation] CancellationToken cancellationToken) {
        var storyIdentifierCollection = await httpClientFactory
            .CreateClient(name: nameof(StoryHandler))
            .GetFromJsonAsync<uint[]>(requestUri: "v0/beststories.json", cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        foreach (var storyIdentifier in storyIdentifierCollection!) {
            if (cancellationToken.IsCancellationRequested) {
                yield break;
            }

            yield return storyIdentifier;
        }
    }
}
