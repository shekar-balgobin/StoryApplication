using MediatR;
using Microsoft.Extensions.Options;
using Santander.Collections.Generic;
using Santander.StoryApplication.WebApplication.Options;

namespace Santander.StoryApplication.WebApplication.Story.BackgroundTask;

internal class PeriodicUpdateBackgroundService :
    AbstractPeriodicBackgroundService {
    public PeriodicUpdateBackgroundService(BufferedMemoryCache<uint, ViewModel.Story> bufferedMemoryCache, ILogger<PeriodicUpdateBackgroundService> logger, IMediator mediator, IOptionsMonitor<PeriodicTimerOptions> optionsMonitor) :
        base(logger, mediator, optionsMonitor.Get(name: nameof(PeriodicUpdateBackgroundService))) {
        bufferedMemoryCache.Toggled += BufferedMemoryCache_Toggled;
    }

    private void BufferedMemoryCache_Toggled(object? sender, EventArgs e) {
        Logger.LogDebug(message: "Refreshed");
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken) {
        do {
            Logger.LogDebug(message: "{Now}: {BackgroundService}", DateTime.UtcNow, nameof(PeriodicUpdateBackgroundService));
        } while (await PeriodicTimer.Value.WaitForNextTickAsync(stoppingToken).ConfigureAwait(continueOnCapturedContext: false));
    }
}
