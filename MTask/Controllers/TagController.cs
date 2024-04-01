using Microsoft.AspNetCore.Mvc;
using MTask.Data.Entities;
using MTask.Services;
using System.Text.Json;

namespace MTask.Controllers
{
    [ApiController]
    [Route("Tag")]
    public class TagController : ControllerBase
    {
        private readonly ILogger<TagController> _logger;
        private readonly HttpClient _httpClient;
        private readonly TagService _tagService;

        public TagController(ILogger<TagController> logger, HttpClient httpClient, TagService tagService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _tagService = tagService;
        }

        [HttpGet("AllTags")]
        public async Task<IActionResult> GetAllTagsAsync()
        {
            int totalPages = 11;
            List<Tag> allTags = new List<Tag>();

            try
            {
                for (int page = 1; page <= totalPages; page++)
                {
                    var response = await _httpClient.GetAsync($"https://api.stackexchange.com/2.2/tags?page={page}&pagesize=100&order=desc&sort=popular&filter=!-.9108i2QY.t&site=stackoverflow");
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();
                    var data = await JsonSerializer.DeserializeAsync<Root>(stream);

                    if (data?.Items != null)
                    {
                        allTags.AddRange(data.Items.Select(i => new Tag { Name = i.Name, Count = i.Count }));
                    }
                }

                await _tagService.ProcessTagsAsync(allTags);

                return Ok(allTags);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching tags: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        public class Root
        {
            public List<Tag> Items { get; set; }
        }
    
    }
}

