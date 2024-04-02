using Microsoft.EntityFrameworkCore;
using MTask.Data;
using MTask.Data.Entities;

namespace MTask.Services
{
    public class TagService
    {
        private readonly TagDbContext _dbContext;

        public TagService(TagDbContext dbContext)
        {
            _dbContext = dbContext;
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
