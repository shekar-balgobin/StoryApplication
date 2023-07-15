namespace Santander.StoryApplication.WebApplication.Story.BackgroundTask;

internal sealed class PeriodicRefreshBackgroundService :
    AbstractPeriodicBackgroundService {
    public PeriodicRefreshBackgroundService(ILogger<PeriodicRefreshBackgroundService> logger) :
        base(logger) {
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        return Task.CompletedTask;
    }
}
