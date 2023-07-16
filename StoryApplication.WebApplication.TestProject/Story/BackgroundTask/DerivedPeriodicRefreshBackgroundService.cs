﻿using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Santander.StoryApplication.WebApplication.Options;
using Santander.StoryApplication.WebApplication.Story.BackgroundTask;

namespace Santander.StoryApplication.WebApplication.TestProject.Story.BackgroundTask;

internal sealed class DerivedPeriodicRefreshBackgroundService :
    PeriodicRefreshBackgroundService {
    public DerivedPeriodicRefreshBackgroundService(ILogger<PeriodicRefreshBackgroundService> logger, IMediator mediator, IOptionsMonitor<PeriodicTimerOptions> optionsMonitor) :
        base(logger, mediator, optionsMonitor) {
    }

    public new async Task ExecuteAsync(CancellationToken stoppingToken) => await base.ExecuteAsync(stoppingToken);
}