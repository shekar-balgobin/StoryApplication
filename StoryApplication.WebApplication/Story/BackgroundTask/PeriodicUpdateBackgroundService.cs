using MediatR;
using Microsoft.Extensions.Options;
using Santander.Collections.Generic;
using Santander.StoryApplication.Story.Query;
using Santander.StoryApplication.Update.Query;
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
            var getUpdateQuery = new GetUpdateQuery();
            var update = await Mediator.Send(getUpdateQuery, stoppingToken).ConfigureAwait(continueOnCapturedContext: false);
            if (update is null) {
                continue;
            }

            foreach (var itemIdentifier in update.Items) {
                var getItemByIdentifierQuery = new GetItemByIdentifierQuery {
                    Id = itemIdentifier
                };

                var item = await Mediator.Send(getItemByIdentifierQuery, stoppingToken).ConfigureAwait(continueOnCapturedContext: false);
                if (item is null) {
                    continue;
                }

                if (!item.Type.Equals("story", StringComparison.InvariantCultureIgnoreCase)) {
                    continue;
                }

                var story = new Model.Story {
                    By = item.By,
                    Id = item.Id,
                    Kids = item.Kids,
                    Score = item.Score,
                    Time = item.Time,
                    Title = item.Title,
                    Type = item.Type,
                    Url = item.Url
                };

                Logger.LogDebug(story.ToString());
            }
        } while (await PeriodicTimer.Value.WaitForNextTickAsync(stoppingToken).ConfigureAwait(continueOnCapturedContext: false));
    }
}
