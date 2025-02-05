namespace Domain.Entities
{
    public class UserAction
    {
        public long Id { get; set; }
        public string TelegramUpdate { get; set; } = null!;
        public string? UserState { get; set; }
        public DateTime CreateDate { get; set; }
    }
}