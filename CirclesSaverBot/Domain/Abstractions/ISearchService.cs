using Domain.Models;

namespace Domain.Abstractions
{
    public interface ISearchService
    {
        Task<SearchResult> Search(long ownerUserId, int maxResult = 50, string? query = null, int? offset = null);
    }
}