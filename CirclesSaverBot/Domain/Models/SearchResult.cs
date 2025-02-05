using Domain.Entities;

namespace Domain.Models
{
    public class SearchResult
    {
        public List<TgMediaFile> Pictures { get; private set; }

        public int? CurrentOffset { get; private set; }

        public int? NextOffset { get; private set; }

        public int TotalPage { get; private set; }

        public SearchResult(List<TgMediaFile> pictures, int? nextOffset = null, int totalPage = 0, int? currentOffset = null)
        {
            Pictures = pictures;
            NextOffset = nextOffset;
            TotalPage = totalPage;
            CurrentOffset = currentOffset;
        }

        public static SearchResult GetEmpty()
        {
            return new SearchResult(new List<TgMediaFile>());
        }
    }
}