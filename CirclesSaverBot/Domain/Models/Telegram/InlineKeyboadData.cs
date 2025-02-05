using Domain.Enums;
using Newtonsoft.Json;

namespace Domain.Models.Telegram
{
    public class InlineKeyboadData<T>
    {
        [JsonProperty("t")]
        public InlineKeyboardTypeEnum Type { get; set; }

        [JsonProperty("d")]
        public T Data { get; set; }

        public static InlineKeyboadData<T> Init(InlineKeyboardTypeEnum typeEnum, T data)
        {
            return new InlineKeyboadData<T>
            {
                Data = data,
                Type = typeEnum
            };
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public static string InitJson(InlineKeyboardTypeEnum typeEnum, T data)
        {
            var dto = new InlineKeyboadData<T>
            {
                Data = data,
                Type = typeEnum
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(dto);
        }

        public static InlineKeyboadData<T> FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<InlineKeyboadData<T>>(json);
        }
    }
}