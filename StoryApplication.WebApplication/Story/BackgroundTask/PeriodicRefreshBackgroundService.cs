using MediatR;
using Microsoft.Extensions.Options;
using Santander.Collections.Generic;
using Santander.StoryApplication.Story.Query;
using Santander.StoryApplication.WebApplication.Options;

namespace Santander.StoryApplication.WebApplication.Story.BackgroundTask;

internal class PeriodicRefreshBackgroundService :
    AbstractPeriodicBackgroundService {
    private readonly BufferedMemoryCache<uint, ViewModel.Story> bufferedMemoryCache;

    public PeriodicRefreshBackgroundService(BufferedMemoryCache<uint, ViewModel.Story> bufferedMemoryCache, ILogger<PeriodicRefreshBackgroundService> logger, IMediator mediator, IOptionsMonitor<PeriodicTimerOptions> optionsMonitor) :
        base(logger, mediator, optionsMonitor.Get(name: nameof(PeriodicRefreshBackgroundService))) =>
        this.bufferedMemoryCache = bufferedMemoryCache;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        do {
            var writer = bufferedMemoryCache.Writer;
            var getStoryIdentifierStreamQuery = new GetStoryIdentifierStreamQuery();
            await foreach (var storyIdentifier in Mediator.CreateStream(getStoryIdentifierStreamQuery, stoppingToken).ConfigureAwait(continueOnCapturedContext: false)) {
                var getStoryByIdentifierQuery = new GetStoryByIdentifierQuery {
                    Id = storyIdentifier
                };

                var story = await Mediator.Send(getStoryByIdentifierQuery, stoppingToken).ConfigureAwait(continueOnCapturedContext: false);
                if (story is null) {
                    continue;
                }

                var viewModelStory = new ViewModel.Story {
                    CommentCount = story.Kids?.Length,
                    PostedBy = story.By,
                    Score = story.Score,
                    Time = story.Time is null ? default : DateTimeOffset.FromUnixTimeSeconds((long)story.Time).UtcDateTime,
                    Title = story.Title,
                    Uri = story.Url
                };

                writer.Add(story.Id, viewModelStory);
            }

            bufferedMemoryCache.Toggle();

            Logger.LogInformation("Cache contains {count} new stories", writer.Count);
        } while (await PeriodicTimer.Value.WaitForNextTickAsync(stoppingToken).ConfigureAwait(continueOnCapturedContext: false));
    }
}
