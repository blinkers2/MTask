using Microsoft.AspNetCore.Mvc;
using MTask.Data.Entities;
using MTask.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTask.Controllers
{
    [ApiController]
    [Route("Tag")]
    public class TagController : ControllerBase
    {
        private readonly ILogger<TagController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TagService _tagService;

        public TagController(ILogger<TagController> logger, IHttpClientFactory httpClientFactory, TagService tagService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tagService = tagService;
        }

        [HttpGet("AllTags")]
        public async Task<IActionResult> GetAllTagsAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("StackExchangeClient");
            int totalPages = 11;
            List<Tag> allTags = new List<Tag>();

            try
            {
                for (int page = 1; page <= totalPages; page++)
                {
                    var response = await httpClient.GetAsync($"tags?page={page}&pagesize=100&order=desc&sort=popular&site=stackoverflow&filter=!bMsg5CXICdlFSp");
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<Root>(jsonResponse);

                    if (data?.Items != null)
                    {
                        allTags.AddRange(data.Items);   
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
            [JsonPropertyName("items")]
            public List<Tag> Items { get; set; }
        }
    
    }
}

