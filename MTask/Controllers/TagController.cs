using Microsoft.AspNetCore.Mvc;
using MTask.Services;

namespace MTask.Controllers
{
    [ApiController]
    [Route("Tag")]
    public class TagController : ControllerBase
    {
        private readonly ILogger<TagController> _logger;
        private readonly TagService _tagService;


        public TagController(ILogger<TagController> logger, TagService tagService)
        {
            _logger = logger;
            _tagService = tagService;
        }
        /// <summary>
        /// Fetches and saves 1100 TAG data from API StackExchange. Add the percentage of each Tag count in whole Tag count population.
        /// </summary>
        [HttpPost("FetchandSaveTags")]
        public async Task<IActionResult> FetchAndSaveTags()
        {
            try
            {
                var tags = await _tagService.FetchTagsFromApiAndSaveAsync();
                await _tagService.ProcessTagsAsync(tags);

                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching and saving tags: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Returns paged and sorted Tag list.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size (tag per page).</param>
        /// <param name="sortBy">Sort by: type "name" to be sorted by name or "percentage" to by sorted by Tag count percentage over all Tags count.</param>
        /// <param name="sortAscending">Sorting ascending or descending.</param>
        [HttpGet("SortByTags")]
        public async Task<IActionResult> GetTagsPaged(
        [FromQuery] int pageNumber = 55,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "percentage",
        [FromQuery] bool sortAscending = true)
        {
            var tags = await _tagService.GetSortedAndPagedTags(pageNumber, pageSize, sortBy, sortAscending);
            return Ok(tags);
        }
        /// <summary>
        /// Refreshes all Tag list by deleting old and fetching current one.
        /// </summary>
        [HttpPost("RefreshTags")]
        public async Task<IActionResult> RefreshTags()
        {
            try
            {
                await _tagService.RemoveTagsAsync();
                var tags = await _tagService.FetchTagsFromApiAndSaveAsync();
                await _tagService.ProcessTagsAsync(tags);

                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching and saving tags: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}

