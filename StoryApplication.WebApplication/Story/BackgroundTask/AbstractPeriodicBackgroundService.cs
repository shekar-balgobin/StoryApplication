using MediatR;
using Santander.StoryApplication.WebApplication.Options;

namespace Santander.StoryApplication.WebApplication.Story.BackgroundTask;

internal abstract class AbstractPeriodicBackgroundService :
    BackgroundService {
    protected readonly ILogger<AbstractPeriodicBackgroundService> Logger;

    protected readonly IMediator Mediator;

    protected Lazy<PeriodicTimer> PeriodicTimer;

    protected AbstractPeriodicBackgroundService(ILogger<AbstractPeriodicBackgroundService> logger, IMediator mediator, PeriodicTimerOptions periodicTimerOptions) {
        (Logger, Mediator) = (logger, mediator);
        PeriodicTimer = new Lazy<PeriodicTimer>(() => new PeriodicTimer(periodicTimerOptions.Period));
    }

    public override async Task StartAsync(CancellationToken cancellationToken) {
        Logger.LogInformation(message: nameof(StartAsync));

        await base.StartAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }

    public override async Task StopAsync(CancellationToken cancellationToken) {
        Logger.LogInformation(message: nameof(StopAsync));

        await base.StopAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }
}
