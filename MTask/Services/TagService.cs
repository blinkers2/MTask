using Microsoft.EntityFrameworkCore;
using MTask.Data;
using MTask.Data.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MTask.Services
{
    public class TagService
    {
        private readonly TagDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public TagService(TagDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<Tag>> FetchTagsFromApiAndSaveAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("StackExchangeClient");
            int totalPages = 11; 
            List<Tag> allTags = new List<Tag>();

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

            return allTags;
        }

        private class Root
        {
            [JsonPropertyName("items")]
            public List<Tag> Items { get; set; }
        }

        public async Task ProcessTagsAsync(List<Tag> allTags)
        {
            decimal totalCounts = allTags.Sum(tag => tag.Count);
            allTags.ForEach(tag => tag.PercentageInWholePopulation = CalculatePercentage(tag.Count, totalCounts));

            _dbContext.Tags.AddRange(allTags);
            await _dbContext.SaveChangesAsync();
        }

        private decimal CalculatePercentage(int part, decimal allTagsSum)
        {
            if (allTagsSum == 0) return 0;
            decimal percentage = (decimal)part / allTagsSum * 100;
            return Math.Round(percentage, 2);
        }

        public async Task<List<Tag>> GetSortedAndPagedTags(int pageNumber, int pageSize, string sortBy, bool sortAscending)
        {
            IQueryable<Tag> query = _dbContext.Tags;

            query = sortBy switch
            {
                "name" => sortAscending ? query.OrderBy(t => t.Name) : query.OrderByDescending(t => t.Name),
                "percentage" => sortAscending ? query.OrderBy(t => t.PercentageInWholePopulation) : query.OrderByDescending(t => t.PercentageInWholePopulation),
                _ => query.OrderBy(t => t.Name)
            };

            var tags = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return tags;
        }

        public async Task RemoveTagsAsync()
        {
            _dbContext.Tags.RemoveRange(_dbContext.Tags);
            await _dbContext.SaveChangesAsync();
        }
    }
}
