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
    }
}
