using Microsoft.EntityFrameworkCore;
using MTask.Data;
using MTask.Data.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MTask.Services
{
    public class TagService : ITagService
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
            int totalPages = 11;
            List<Tag> allTags = new List<Tag>();

            for (int page = 1; page <= totalPages; page++)
            {
                string requestUri = $"tags?page={page}&pagesize=100&order=desc&sort=popular&site=stackoverflow&filter=!bMsg5CXICdlFSp";
                var tagsFromPage = await GetTagsFromApiAsync(requestUri);
                allTags.AddRange(tagsFromPage);
            }

            return allTags;
        }

        protected virtual async Task<List<Tag>> GetTagsFromApiAsync(string requestUri)
        {
            var httpClient = _httpClientFactory.CreateClient("StackExchangeClient");
            var response = await httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Root>(jsonResponse);

            return data?.Items ?? new List<Tag>();
        }

        private class Root // to mogę dać do folderu MODELS jak go stworzę...
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

        private decimal CalculatePercentage(int singleTagCount, decimal allTagsSum) =>
            allTagsSum == 0 ? 0 : Math.Round((decimal)singleTagCount / allTagsSum * 100, 2);

        public async Task<List<Tag>> GetSortedAndPagedTags(int pageNumber, int pageSize, string sortBy, bool sortAscending) // to jest paginacja, przerzucona na baze danych
            // czy mam to dać jako oddzielną klasę do folderu MODELS, tak mi się wydawało że bez sensu...
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
            // Tu ważne bo zwracam całe TAG data z bazy danych razem z Id, mam zrobić DTO żeby tylko zwracać name, count i percentage? Id jest zrobione przeze mnie z GUID
            return tags;
        }

        public async Task RemoveTagsAsync()
        {
            _dbContext.Tags.RemoveRange(_dbContext.Tags);
            await _dbContext.SaveChangesAsync();
        }
    }
}