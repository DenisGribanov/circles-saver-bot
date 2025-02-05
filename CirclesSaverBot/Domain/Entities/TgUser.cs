namespace Domain.Entities
{
    public class TgUser
    {
        public long ChatId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<TgMediaFile> Pictures { get; set; } = new HashSet<TgMediaFile>();
        public ICollection<InlineResultStatistics> PictureClickStatistics { get; set; } = new HashSet<InlineResultStatistics>();
    }
}