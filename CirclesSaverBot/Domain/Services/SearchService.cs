using Domain.Abstractions;
using Domain.Models;

namespace Domain.Services
{
    public class SearchService : ISearchService
    {
        private readonly IDataStore _picSaverDataStore;

        public SearchService(IDataStore picSaverDataStore)
        {
            _picSaverDataStore = picSaverDataStore;
        }

        public async Task<SearchResult> Search(long ownerUserId, int maxResult = 50, string? query = null, int? offset = null)
        {
            var pictures = await _picSaverDataStore.GetTgMediaFiles(ownerUserId);

            var filterPictures = pictures
                    .Where(x => x.IsVisable && !x.IsDeleted &&
                                (string.IsNullOrEmpty(query)
                                || x.Description != null && x.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
                                || int.TryParse(query, out var number) && x.Number == number)
                                )
                    .OrderByDescending(x => x.Id).ToList();

            int currentPage = offset.HasValue ? offset.Value : 0;
            int searhResultCount = filterPictures.Count;
            int totalPageCount = GetTotalPageCount(maxResult, searhResultCount);

            if (searhResultCount == 0) return SearchResult.GetEmpty();

            int indexStart = GetStartIndexRange(currentPage, maxResult);

            if (indexStart > searhResultCount) return SearchResult.GetEmpty();

            int rangeCount = GetRangeCount(indexStart, searhResultCount);
            int? nextPageIndex = GetNextPageIndex(currentPage, maxResult, searhResultCount);

            var result = filterPictures.GetRange(indexStart, rangeCount).Take(maxResult).ToList();

            int? nextOffset = nextPageIndex.HasValue ? nextPageIndex : null;

            return new SearchResult(result, nextOffset, totalPageCount, offset);
        }

        private static int GetTotalPageCount(int pageSize, int elementAmount)
        {
            if (pageSize == 0 || elementAmount == 0) { return 0; }

            var result = Math.Round((decimal)elementAmount / pageSize, MidpointRounding.ToPositiveInfinity);

            return (int)result;
        }

        private static int GetStartIndexRange(int? currentPage, int pageSize)
        {
            if (!currentPage.HasValue)
            {
                return 0;
            }
            else if (currentPage.Value == 0)
            {
                return currentPage.Value * pageSize;
            }
            else
            {
                return (currentPage.Value - 1) * pageSize;
            }
        }

        private static int GetRangeCount(int indexStart, int arraySize)
        {
            return indexStart == 0 ?
                arraySize - indexStart :
                arraySize - indexStart;
        }

        private static int? GetNextPageIndex(int currentPageIndex, int pageSize, int arraySize)
        {
            if (currentPageIndex * pageSize + pageSize >= arraySize)
            {
                return null;
            }
            else
            {
                return currentPageIndex += 1;
            }
        }
    }
}