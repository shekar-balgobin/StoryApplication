using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace Santander.StoryApplication.WebApplication.Controller;

/// <summary>
/// Best stories.
/// </summary>
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route(template: "api/")]
public sealed class StoryController :
    ControllerBase {
    /// <summary>
    /// The first n 'best stories' from Hacker News API, sorted by score in descending order.
    /// </summary>
    /// <param name="n">Number of stories.</param>
    /// <returns></returns>
    [HttpGet("story")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ViewModel.Story>))]
    public async Task<IActionResult> Get([Range(0, ushort.MaxValue)] int n) {
        var storyCollection = Enumerable.Range(0, n).Select(i => new ViewModel.Story {
            Score = (uint)i
        });

        await Task.Delay(0);

        return Ok(storyCollection);
    }
}
