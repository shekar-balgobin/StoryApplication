using AutoFixture.Xunit2;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Santander.Collections.Generic;
using Santander.StoryApplication.WebApplication.Options;
using Santander.StoryApplication.WebApplication.Story.BackgroundTask;

namespace Santander.StoryApplication.WebApplication.TestProject.Story.BackgroundTask;

public sealed class PeriodicRefreshBackgroundServiceTest {
    private readonly BufferedMemoryCache<uint, ViewModel.Story> bufferedMemoryCache = new();

    private readonly CancellationTokenSource cancellationTokenSource = new();

    private readonly ILogger<PeriodicRefreshBackgroundService> logger = Mock.Of<ILogger<PeriodicRefreshBackgroundService>>();

    private readonly Mock<IMediator> mediator = new();

    private readonly Mock<IOptionsMonitor<PeriodicTimerOptions>> optionsMonitor = new();

    [InlineAutoData(1)]
    [Theory]
    public async Task ExecuteAsync(byte delaySeconds, byte periodMilliseconds, IEnumerable<uint> storyIdentifierCollection) {
        var cancellationToken = cancellationTokenSource.Token;
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(delaySeconds));

        mediator.Setup(m => m.CreateStream(It.IsAny<IStreamRequest<uint>>(), It.IsAny<CancellationToken>())).Returns(ToAsyncEnumerable(storyIdentifierCollection));

        optionsMonitor.Setup(om => om.Get(It.IsAny<string>())).Returns(new PeriodicTimerOptions {
            Period = TimeSpan.FromMilliseconds(periodMilliseconds)
        });

        var periodicRefreshBackgroundService = new DerivedPeriodicRefreshBackgroundService(bufferedMemoryCache, logger, mediator.Object, optionsMonitor.Object);
        var action = async () => await periodicRefreshBackgroundService.ExecuteAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

        await action.Should().ThrowAsync<OperationCanceledException>();
        cancellationToken.IsCancellationRequested.Should().BeTrue();
    }

    private static async IAsyncEnumerable<uint> ToAsyncEnumerable(IEnumerable<uint> collection) {
        foreach (var item in collection) {
            yield return item;
        }

        await Task.Delay(0);
    }
}
