using MTask.Data.Entities;

namespace MTask.Services
{
    public interface ITagService
    {
        Task<List<Tag>> FetchTagsFromApiAndSaveAsync();
        Task<List<Tag>> GetSortedAndPagedTags(int pageNumber, int pageSize, string sortBy, bool sortAscending);
        Task ProcessTagsAsync(List<Tag> allTags);
        Task RemoveTagsAsync();
    }
}