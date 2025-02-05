namespace Domain.Entities
{
    public class InlineResultStatistics
    {
        public long Id { get; set; }

        public long TgMediaFileId { get; set; }

        public long TgUserId { get; set; }

        public int ClickCount { get; set; }

        public TgUser TgUser { get; set; }

        public TgMediaFile TgMediaFile { get; set; }
    }
}