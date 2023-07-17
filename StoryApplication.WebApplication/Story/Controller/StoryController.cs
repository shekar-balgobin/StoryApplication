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

    private readonly ConcurrentMemoryCache<uint, ViewModel.Story> concurrentMemoryCache;

    private readonly ILogger<StoryController> logger;

    /// <summary>
    /// In memory story cache.
    /// </summary>
    /// <param name="bufferedMemoryCache"></param>
    /// <param name="concurrentMemoryCache"></param>
    /// <param name="logger"></param>
    public StoryController(BufferedMemoryCache<uint, ViewModel.Story> bufferedMemoryCache, ConcurrentMemoryCache<uint, ViewModel.Story> concurrentMemoryCache, ILogger<StoryController> logger) =>
        (this.bufferedMemoryCache, this.concurrentMemoryCache, this.logger) = (bufferedMemoryCache, concurrentMemoryCache, logger);

    /// <summary>
    /// The first n 'best stories' from Hacker News API, sorted by score in descending order.
    /// </summary>
    /// <param name="n">Number of stories.</param>
    /// <returns></returns>
    [HttpGet("story")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ViewModel.Story>))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Get([Range(minimum: 0, maximum: ushort.MaxValue)] int n = 1) {
        if (!bufferedMemoryCache.Reader.Any()) {
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        var bufferedMemoryCacheCopy = bufferedMemoryCache.Reader.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        var minimumScore = bufferedMemoryCacheCopy.Values.Min(s => s.Score);
        var updateCount = default(int);
        foreach (var keyValuePair in concurrentMemoryCache.Reader.Where(kvp => kvp.Value.Score >= minimumScore)) {
            if (bufferedMemoryCacheCopy.Remove(keyValuePair.Key)) {
                bufferedMemoryCacheCopy.Add(keyValuePair.Key, keyValuePair.Value);
                updateCount++;
            }
        }

        logger.LogInformation("{UpdateCount} story updates were applied", updateCount);

        return Ok(bufferedMemoryCacheCopy.OrderByDescending(s => s.Value.Score).Select(kvp => kvp.Value).Take(count: n));
    }
}
