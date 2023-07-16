using MediatR;
using Microsoft.Extensions.Options;
using Santander.StoryApplication.WebApplication.Options;

namespace Santander.StoryApplication.WebApplication.Story.BackgroundTask;

internal class PeriodicRefreshBackgroundService :
    AbstractPeriodicBackgroundService {
    public PeriodicRefreshBackgroundService(ILogger<PeriodicRefreshBackgroundService> logger, IMediator mediator, IOptionsMonitor<PeriodicTimerOptions> optionsMonitor) :
        base(logger, mediator, optionsMonitor.Get(name: nameof(PeriodicRefreshBackgroundService))) {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        do {
            Logger.LogDebug(message: "{Now}: {BackgroundService}", DateTime.UtcNow, nameof(PeriodicRefreshBackgroundService));
        } while (await PeriodicTimer.Value.WaitForNextTickAsync(stoppingToken).ConfigureAwait(continueOnCapturedContext: false));
    }
}
