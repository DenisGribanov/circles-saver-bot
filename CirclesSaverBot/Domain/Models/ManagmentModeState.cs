using Domain.Enums;

namespace Domain.Models
{
    public class ManagmentModeState
    {
        public ManagmentActionEnum Action { get; set; }
        public long TgMediaFileId { get; set; }
    }
}