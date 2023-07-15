using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Santander.Collections.Generic;
using Santander.StoryApplication.WebApplication.Story.Controller;

namespace Santander.StoryApplication.WebApplication.TestProject.Story.Controller;

public sealed class StoryControllerTest {
    [AutoData]
    [Theory]
    public void Get(IEnumerable<ViewModel.Story> storyCollection) {
        var bufferedMemoryCache = new BufferedMemoryCache<uint, ViewModel.Story>();
        var writer = bufferedMemoryCache.Writer;

        var id = default(uint);
        foreach (var story in storyCollection) {
            writer.Add(id++, story);
        }

        bufferedMemoryCache.Toggle();

        var storyController = new StoryController(bufferedMemoryCache);
        var actionResult = storyController.Get(int.MaxValue);

        actionResult.Should().BeOfType<OkObjectResult>();

        var actual = ((OkObjectResult)actionResult).Value as IEnumerable<ViewModel.Story>;
        actual.Should().BeEquivalentTo(storyCollection);
    }
}
