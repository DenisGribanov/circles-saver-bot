namespace Domain.Entities
{
    public class TgMediaFile
    {
        public long Id { get; set; }
        public long Number { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string FileId { get; set; } = null!;

        public string FileUniqueId { get; set; } = null!;

        public long FileSize { get; set; }

        public int VideoDuration { get; set; }

        public string? Description { get; set; }

        public string[]? HashTags { get; set; }

        public long OwnerTgUserId { get; set; }

        public TgUser TgUserOwner { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsVisable { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public ICollection<InlineResultStatistics> ClickStatistics { get; set; } = new List<InlineResultStatistics>();
    }
}