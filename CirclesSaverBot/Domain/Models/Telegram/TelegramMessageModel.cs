using Domain.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Update = Telegram.Bot.Types.Update;

namespace Domain.Models.Telegram
{
    public class TelegramMessageModel
    {
        /// <summary>
        /// id отправителя
        /// </summary>
        public long UserFromId { get; set; }

        public string? Username { get; set; }
        public string? MessageText { get; set; }
        public string? Caption { get; set; }
        public bool IsChosenInlineResult { get; set; }
        public string? ChosenInlineResultId { get; set; }
        public string? InlineQueryText { get; set; }
        public string? InlineQueryId { get; set; }
        public string? InlineQueryOffset { get; set; }
        public bool IsInlineQuery { get; set; }
        public bool IsCallBackQuery { get; set; }
        public string? CallBackData { get; set; }

        public string? CallBackQueryId { get; set; }

        public TelegramFileInfoModel? TelegramFile { get; set; }

        public string SourceUpdateMessageJson { get; set; }

        public static TelegramMessageModel Init(Update update)
        {
            if (update == null) throw new ArgumentNullException(nameof(update));

            TelegramMessageModel model = new TelegramMessageModel
            {
                UserFromId = GetUserFromId(update),
                Username = GetUserName(update),
                MessageText = GetMessage(update)?.Text,
                IsInlineQuery = update.Type.Equals(UpdateType.InlineQuery),
                InlineQueryText = update?.InlineQuery?.Query,
                ChosenInlineResultId = update.ChosenInlineResult?.ResultId,
                InlineQueryOffset = update?.InlineQuery?.Offset,
                IsChosenInlineResult = update.Type == UpdateType.ChosenInlineResult,
                TelegramFile = InitTelegramFileInfoModel(update),
                InlineQueryId = update?.InlineQuery?.Id,
                Caption = update?.Message?.Caption,
                IsCallBackQuery = update.CallbackQuery != null,
                CallBackData = update?.CallbackQuery?.Data,
                CallBackQueryId = update?.CallbackQuery?.Id,
                SourceUpdateMessageJson = Newtonsoft.Json.JsonConvert.SerializeObject(update)
            };

            return model;
        }

        private static TelegramFileInfoModel? InitTelegramFileInfoModel(Update update)
        {
            var msg = GetMessage(update);

            if (msg == null) return null;

            if (msg.VideoNote != null)
            {
                TelegramFileInfoModel model = new TelegramFileInfoModel
                {
                    FileId = msg.VideoNote.FileId,
                    FileSize = msg.VideoNote.FileSize,
                    FileType = TelegramFileTypeEnum.VideoNote,
                    FileUniqueId = msg.VideoNote.FileUniqueId,
                    VideoDuration = msg.VideoNote.Duration,
                };

                return model;
            }
            else if (msg.Video != null)
            {
                TelegramFileInfoModel model = new TelegramFileInfoModel
                {
                    FileId = msg.Video.FileId,
                    FileSize = msg.Video.FileSize,
                    FileType = TelegramFileTypeEnum.Video,
                    FileUniqueId = msg.Video.FileUniqueId,
                    VideoDuration = msg.Video.Duration,
                };

                return model;
            }
            else if (msg.Document != null)
            {
                TelegramFileInfoModel model = new TelegramFileInfoModel
                {
                    FileId = msg.Document.FileId,
                    FileSize = msg.Document.FileSize,
                    FileType = TelegramFileTypeEnum.File,
                    FileUniqueId = msg.Document.FileUniqueId
                };

                return model;
            }

            return null;
        }

        private static long GetUserFromId(Update update)
        {
            if (update.Message != null && update.Message.From != null)
            {
                return update.Message.From.Id;
            }
            else if (update.ChosenInlineResult != null && update.ChosenInlineResult.From != null)
            {
                return update.ChosenInlineResult.From.Id;
            }
            else if (update.InlineQuery != null && update.InlineQuery.From != null)
            {
                return update.InlineQuery.From.Id;
            }
            else if (update.CallbackQuery != null)
            {
                return update.CallbackQuery.From.Id;
            }
            else if (update.MyChatMember != null)
            {
                return update.MyChatMember.From.Id;
            }
            else
            {
                return 0;
            }
        }

        private static string GetUserName(Update update)
        {
            Message message = GetMessage(update);

            var name = message != null ? message.Chat?.Username : update.InlineQuery?.From.Username;

            if (update.CallbackQuery != null)
            {
                name = update.CallbackQuery.From.Username;
            }
            else if (update.ChosenInlineResult != null)
            {
                name = update.ChosenInlineResult.From?.Username;
            }

            return string.IsNullOrEmpty(name) ? "UNKNOWN" : name;
        }

        private static Message? GetMessage(Update update)
        {
            if (update.Message != null)
                return update.Message;
            else if (update.ChannelPost != null && update.Message != null)
                return update.ChannelPost;
            else if (update.EditedMessage != null)
                return update.EditedMessage;
            else if (update.EditedChannelPost != null)
                return update.EditedChannelPost;
            else
                return null;
        }
    }

    public class TelegramFileInfoModel
    {
        public TelegramFileTypeEnum? FileType { get; set; }
        public string FileId { get; set; } = null!;

        public string FileUniqueId { get; set; } = null!;

        public long? FileSize { get; set; }

        public int VideoDuration { get; set; }
    }
}