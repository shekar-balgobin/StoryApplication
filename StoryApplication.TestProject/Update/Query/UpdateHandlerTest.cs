using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using RichardSzalay.MockHttp;
using Santander.StoryApplication.Update.Query;
using System.Net.Mime;
using System.Text.Json;

namespace Santander.StoryApplication.TestProject.Update.Query;

public sealed class UpdateHandlerTest {
    private const string BaseAddress = "https://hacker-news.firebaseio.com/";

    private readonly CancellationTokenSource cancellationTokenSource = new();

    [AutoData]
    [Theory]
    public async Task Handle_GetItemByIdentifierQuery(Model.Item item) {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler.When(url: $"{BaseAddress}v0/item/{item.Id}.json").Respond(MediaTypeNames.Application.Json, JsonSerializer.Serialize(item));

        var httpClient = mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri(BaseAddress);

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(hcf => hcf.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var getItemByIdentifierQuery = new GetItemByIdentifierQuery {
            Id = item.Id
        };

        var itemHandler = new UpdateHandler(httpClientFactory.Object);
        var actual = await itemHandler.Handle(getItemByIdentifierQuery, cancellationTokenSource.Token);
        actual.Should().BeEquivalentTo(item);
    }

    [AutoData]
    [Theory]
    public async Task Handle_GetUpdateQuery(Model.Update update) {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler.When(url: $"{BaseAddress}v0/updates.json").Respond(MediaTypeNames.Application.Json, JsonSerializer.Serialize(update));

        var httpClient = mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri(BaseAddress);

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(hcf => hcf.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var getUpdateQuery = new GetUpdateQuery();

        var itemHandler = new UpdateHandler(httpClientFactory.Object);
        var actual = await itemHandler.Handle(getUpdateQuery, cancellationTokenSource.Token);
        actual.Should().BeEquivalentTo(update);
    }
}
