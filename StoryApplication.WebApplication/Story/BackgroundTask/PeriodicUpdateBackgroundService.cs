namespace Santander.StoryApplication.WebApplication.Story.BackgroundTask;

internal class PeriodicUpdateBackgroundService :
    AbstractPeriodicBackgroundService {
    public PeriodicUpdateBackgroundService(ILogger<PeriodicUpdateBackgroundService> logger) :
        base(logger) {
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        return Task.CompletedTask;
    }
}
