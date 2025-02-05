using Domain.Enums;
using Newtonsoft.Json;

namespace Domain.Models.Telegram
{
    public class InlineManagmentMode
    {
        [JsonProperty("a")]
        public ManagmentActionEnum Action { get; set; }

        [JsonProperty("id")]
        public long TgMediaFileId { get; set; }

        public static InlineManagmentMode Init(ManagmentActionEnum action, long id)
        {
            return new InlineManagmentMode
            {
                Action = action,
                TgMediaFileId = id
            };
        }
    }
}