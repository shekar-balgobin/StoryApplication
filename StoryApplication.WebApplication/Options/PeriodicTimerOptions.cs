namespace Santander.StoryApplication.WebApplication.Options;

internal sealed record class PeriodicTimerOptions {
    public required TimeSpan Period { get; init; }
}
