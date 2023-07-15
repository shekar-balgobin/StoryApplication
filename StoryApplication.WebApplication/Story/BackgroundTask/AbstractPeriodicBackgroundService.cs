namespace Santander.StoryApplication.WebApplication.Story.BackgroundTask;

internal abstract class AbstractPeriodicBackgroundService :
    BackgroundService {
    protected readonly ILogger<AbstractPeriodicBackgroundService> Logger;

    protected AbstractPeriodicBackgroundService(ILogger<AbstractPeriodicBackgroundService> logger) =>
        Logger = logger;

    public override async Task StartAsync(CancellationToken cancellationToken) {
        Logger.LogInformation(message: nameof(StartAsync));

        await base.StartAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }

    public override async Task StopAsync(CancellationToken cancellationToken) {
        Logger.LogInformation(message: nameof(StopAsync));

        await base.StopAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }
}
