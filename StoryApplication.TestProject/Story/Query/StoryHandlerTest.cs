using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using RichardSzalay.MockHttp;
using Santander.StoryApplication.Story.Query;
using System.Net.Mime;
using System.Text.Json;

namespace Santander.StoryApplication.TestProject.Story.Query;

public sealed class StoryHandlerTest {
    private const string BaseAddress = "https://hacker-news.firebaseio.com/";

    private readonly CancellationTokenSource cancellationTokenSource = new();

    [AutoData]
    [Theory]
    public async Task Handle_GetStoryByIdentifierQuery(Model.Story story) {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler.When(url: $"{BaseAddress}v0/item/{story.Id}.json").Respond(MediaTypeNames.Application.Json, JsonSerializer.Serialize(story));

        var httpClient = mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri(BaseAddress);

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(hcf => hcf.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var getStoryByIdentifierQuery = new GetStoryByIdentifierQuery {
            Id = story.Id
        };

        var storyHandler = new StoryHandler(httpClientFactory.Object);
        var actual = await storyHandler.Handle(getStoryByIdentifierQuery, cancellationTokenSource.Token);
        actual.Should().BeEquivalentTo(story);
    }

    [AutoData]
    [Theory]
    public async Task Handle_GetStoryIdentifierStreamQuery(uint[] storyIdentifierCollection) {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler.When(url: $"{BaseAddress}v0/beststories.json").Respond(MediaTypeNames.Application.Json, JsonSerializer.Serialize(storyIdentifierCollection));

        var httpClient = mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri(BaseAddress);

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(hcf => hcf.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var getStoryIdentifierStreamQuery = new GetStoryIdentifierStreamQuery();
        var storyHandler = new StoryHandler(httpClientFactory.Object);
        await foreach (var storyIdentifier in storyHandler.Handle(getStoryIdentifierStreamQuery, cancellationTokenSource.Token)) {
            storyIdentifierCollection.Should().Contain(storyIdentifier);
        };
    }

    [AutoData]
    [Theory]
    public async Task Handle_GetStoryIdentifierStreamQuery_Cancel(uint[] storyIdentifierCollection) {
        var cancellationToken = cancellationTokenSource.Token;

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler.When(url: $"{BaseAddress}v0/beststories.json").Respond(MediaTypeNames.Application.Json, JsonSerializer.Serialize(storyIdentifierCollection));

        var httpClient = mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri(BaseAddress);

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(hcf => hcf.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var getStoryIdentifierStreamQuery = new GetStoryIdentifierStreamQuery();
        var storyHandler = new StoryHandler(httpClientFactory.Object);
        await foreach (var storyIdentifier in storyHandler.Handle(getStoryIdentifierStreamQuery, cancellationToken)) {
            cancellationTokenSource.Cancel();
        };

        cancellationTokenSource.IsCancellationRequested.Should().BeTrue();
    }
}
