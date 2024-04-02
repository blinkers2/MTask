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

