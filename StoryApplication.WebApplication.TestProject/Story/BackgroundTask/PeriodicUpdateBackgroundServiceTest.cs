using AutoFixture.Xunit2;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Santander.StoryApplication.WebApplication.Options;
using Santander.StoryApplication.WebApplication.Story.BackgroundTask;

namespace Santander.StoryApplication.WebApplication.TestProject.Story.BackgroundTask;

public sealed class PeriodicUpdateBackgroundServiceTest {
    private readonly CancellationTokenSource cancellationTokenSource = new();

    private readonly ILogger<PeriodicUpdateBackgroundService> logger = Mock.Of<ILogger<PeriodicUpdateBackgroundService>>();

    private readonly IMediator mediator = Mock.Of<IMediator>();

    private readonly Mock<IOptionsMonitor<PeriodicTimerOptions>> optionsMonitor = new();

    [InlineAutoData(1)]
    [Theory]
    public async Task ExecuteAsync(byte delaySeconds, byte periodMilliseconds) {
        var cancellationToken = cancellationTokenSource.Token;
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(delaySeconds));

        optionsMonitor.Setup(om => om.Get(It.IsAny<string>())).Returns(new PeriodicTimerOptions {
            Period = TimeSpan.FromMilliseconds(periodMilliseconds)
        });

        var periodicUpdateBackgroundService = new DerivedPeriodicUpdateBackgroundService(logger, mediator, optionsMonitor.Object);
        var action = async () => await periodicUpdateBackgroundService.ExecuteAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

        await action.Should().ThrowAsync<OperationCanceledException>();
        cancellationToken.IsCancellationRequested.Should().BeTrue();
    }
}
