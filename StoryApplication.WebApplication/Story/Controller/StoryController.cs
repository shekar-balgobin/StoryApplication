using Microsoft.AspNetCore.Mvc;
using Santander.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace Santander.StoryApplication.WebApplication.Story.Controller;

/// <summary>
/// Best stories.
/// </summary>
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route(template: "api/")]
public sealed class StoryController :
    ControllerBase {
    private readonly BufferedMemoryCache<uint, ViewModel.Story> bufferedMemoryCache;

    /// <summary>
    /// In memory story cache.
    /// </summary>
    /// <param name="bufferedMemoryCache"></param>
    public StoryController(BufferedMemoryCache<uint, ViewModel.Story> bufferedMemoryCache) =>
        this.bufferedMemoryCache = bufferedMemoryCache;

    /// <summary>
    /// The first n 'best stories' from Hacker News API, sorted by score in descending order.
    /// </summary>
    /// <param name="n">Number of stories.</param>
    /// <returns></returns>
    [HttpGet("story")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ViewModel.Story>))]
    public IActionResult Get([Range(0, ushort.MaxValue)] int n) => Ok(bufferedMemoryCache.Reader.Values.Take(count: n));
}
