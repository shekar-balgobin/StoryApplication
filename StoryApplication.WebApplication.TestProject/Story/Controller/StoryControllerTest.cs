using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Santander.Collections.Generic;
using Santander.StoryApplication.WebApplication.Story.Controller;

namespace Santander.StoryApplication.WebApplication.TestProject.Story.Controller;

public sealed class StoryControllerTest {
    [AutoData]
    [Theory]
    public void Get(Dictionary<uint, ViewModel.Story> storyCollection) {
        var bufferedMemoryCache = new BufferedMemoryCache<uint, ViewModel.Story>();
        var writer = bufferedMemoryCache.Writer;

        foreach (var story in storyCollection) {
            writer.Add(story.Key, story.Value);
        }

        bufferedMemoryCache.Toggle();

        var storyController = new StoryController(bufferedMemoryCache);
        var actionResult = storyController.Get(int.MaxValue);

        actionResult.Should().BeOfType<OkObjectResult>();

        var actual = ((OkObjectResult)actionResult).Value as IEnumerable<ViewModel.Story>;
        actual.Should().BeEquivalentTo(storyCollection.Values);
    }

    [Fact]
    public void Get_Status503ServiceUnavailable() {
        var bufferedMemoryCache = new BufferedMemoryCache<uint, ViewModel.Story>();

        var storyController = new StoryController(bufferedMemoryCache);
        var actionResult = storyController.Get(int.MaxValue);

        actionResult.Should().BeOfType<StatusCodeResult>();

        var actual = ((StatusCodeResult)actionResult).StatusCode;
        actual.Should().Be(StatusCodes.Status503ServiceUnavailable);
    }
}
