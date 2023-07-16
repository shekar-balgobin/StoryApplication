using MediatR;
using Microsoft.Extensions.Options;
using Santander.StoryApplication.WebApplication.Options;

namespace Santander.StoryApplication.WebApplication.Story.BackgroundTask;

internal class PeriodicUpdateBackgroundService :
    AbstractPeriodicBackgroundService {
    public PeriodicUpdateBackgroundService(ILogger<PeriodicUpdateBackgroundService> logger, IMediator mediator, IOptionsMonitor<PeriodicTimerOptions> optionsMonitor) :
        base(logger, mediator, optionsMonitor.Get(name: nameof(PeriodicUpdateBackgroundService))) {
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken) {
        do {
            Logger.LogDebug(message: "{Now}: {BackgroundService}", DateTime.UtcNow, nameof(PeriodicUpdateBackgroundService));
        } while (await PeriodicTimer.Value.WaitForNextTickAsync(stoppingToken).ConfigureAwait(continueOnCapturedContext: false));
    }
}
