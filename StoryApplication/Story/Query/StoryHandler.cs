using MediatR;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace Santander.StoryApplication.Story.Query;

public sealed class StoryHandler :
    IStreamRequestHandler<GetStoryIdentifierStreamQuery, uint> {
    private readonly IHttpClientFactory httpClientFactory;

    public StoryHandler(IHttpClientFactory httpClientFactory) =>
        this.httpClientFactory = httpClientFactory;

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
