using MediatR;
using Microsoft.Extensions.Options;
using Santander.Collections.Generic;
using Santander.StoryApplication.Update.Query;
using Santander.StoryApplication.WebApplication.Options;

namespace Santander.StoryApplication.WebApplication.Story.BackgroundTask;

internal class PeriodicUpdateBackgroundService :
    AbstractPeriodicBackgroundService {
    private readonly BufferedMemoryCache<uint, ViewModel.Story> bufferedMemoryCache;

    private bool? clearCachedStories;

    private readonly ConcurrentMemoryCache<uint, ViewModel.Story> concurrentMemoryCache;

    public PeriodicUpdateBackgroundService(BufferedMemoryCache<uint, ViewModel.Story> bufferedMemoryCache, ConcurrentMemoryCache<uint, ViewModel.Story> concurrentMemoryCache, ILogger<PeriodicUpdateBackgroundService> logger, IMediator mediator, IOptionsMonitor<PeriodicTimerOptions> optionsMonitor) :
        base(logger, mediator, optionsMonitor.Get(name: nameof(PeriodicUpdateBackgroundService))) {
        (this.bufferedMemoryCache, this.concurrentMemoryCache) = (bufferedMemoryCache, concurrentMemoryCache);
        bufferedMemoryCache.Toggled += BufferedMemoryCache_Toggled;
    }

    private void BufferedMemoryCache_Toggled(object? sender, EventArgs e) {
        clearCachedStories = true;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken) {
        do {
            if (!clearCachedStories.HasValue) {
                continue;
            }

            var getUpdateQuery = new GetUpdateQuery();
            var update = await Mediator.Send(getUpdateQuery, stoppingToken).ConfigureAwait(continueOnCapturedContext: false);
            if (update is null) {
                continue;
            }

            var writer = concurrentMemoryCache.Writer;
            if (clearCachedStories.Value) {
                writer.Clear();
                clearCachedStories = false;
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

                var story = new ViewModel.Story {
                    CommentCount = item.Kids?.Length,
                    PostedBy = item.By,
                    Score = item.Score,
                    Time = item.Time is null ? default : DateTimeOffset.FromUnixTimeSeconds((long)item.Time).UtcDateTime,
                    Title = item.Title,
                    Uri = item.Url
                };

                if (!writer.TryAdd(item.Id, story)) {
                    writer.Remove(item.Id);
                    writer.Add(item.Id, story);
                }
            }

            Logger.LogInformation("Cache contains an additional {count} story updates", writer.Count);
        } while (await PeriodicTimer.Value.WaitForNextTickAsync(stoppingToken).ConfigureAwait(continueOnCapturedContext: false));
    }
}
