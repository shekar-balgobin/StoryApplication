using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Santander.Collections.Generic;
using Santander.StoryApplication.WebApplication.Story.Controller;

namespace Santander.StoryApplication.WebApplication.TestProject.Story.Controller;

public sealed class StoryControllerTest {
    private readonly ILogger<StoryController> logger = Mock.Of<ILogger<StoryController>>();

    [AutoData]
    [Theory]
    public void Get(Dictionary<uint, ViewModel.Story> storyCollection) {
        var bufferedMemoryCache = new BufferedMemoryCache<uint, ViewModel.Story>();
        var concurrentMemoryCache = new ConcurrentMemoryCache<uint, ViewModel.Story>();
        var writer = bufferedMemoryCache.Writer;

        foreach (var story in storyCollection) {
            writer.Add(story.Key, story.Value);
        }

        bufferedMemoryCache.Toggle();

        var storyController = new StoryController(bufferedMemoryCache, concurrentMemoryCache, logger);
        var actionResult = storyController.Get(int.MaxValue);

        actionResult.Should().BeOfType<OkObjectResult>();

        var actual = ((OkObjectResult)actionResult).Value as IEnumerable<ViewModel.Story>;
        actual.Should().BeEquivalentTo(storyCollection.Values);
    }

    [AutoData]
    [Theory]
    public void Get_WithUpdates(Dictionary<uint, ViewModel.Story> storyCollection) {
        var bufferedMemoryCache = new BufferedMemoryCache<uint, ViewModel.Story>();
        var concurrentMemoryCache = new ConcurrentMemoryCache<uint, ViewModel.Story>();
        var writer = bufferedMemoryCache.Writer;

        foreach (var story in storyCollection) {
            writer.Add(story);
        }

        bufferedMemoryCache.Toggle();

        var lastKeyValuePair = storyCollection.Last();
        var lastStory = lastKeyValuePair.Value;
        lastStory = lastStory with {
            Score = int.MaxValue
        };

        concurrentMemoryCache.Writer.Add(lastKeyValuePair.Key, lastStory);

        var storyController = new StoryController(bufferedMemoryCache, concurrentMemoryCache, logger);
        var actionResult = storyController.Get(int.MaxValue);

        actionResult.Should().BeOfType<OkObjectResult>();

        var actual = ((OkObjectResult)actionResult).Value as IEnumerable<ViewModel.Story>;
        actual.Should().NotBeEquivalentTo(storyCollection.Values);
    }

    [Fact]
    public void Get_Status503ServiceUnavailable() {
        var bufferedMemoryCache = new BufferedMemoryCache<uint, ViewModel.Story>();
        var concurrentMemoryCache = new ConcurrentMemoryCache<uint, ViewModel.Story>();

        var storyController = new StoryController(bufferedMemoryCache, concurrentMemoryCache, logger);
        var actionResult = storyController.Get(int.MaxValue);

        actionResult.Should().BeOfType<StatusCodeResult>();

        var actual = ((StatusCodeResult)actionResult).StatusCode;
        actual.Should().Be(StatusCodes.Status503ServiceUnavailable);
    }
}
