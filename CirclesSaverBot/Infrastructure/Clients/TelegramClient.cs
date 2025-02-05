using Domain.Abstractions;
using Domain.Entities;
using Domain.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Clients
{
    public class TelegramClient : ITelegramClient
    {
        private readonly ITelegramBotClient _telegramBotClient;

        public TelegramClient(ITelegramBotClient telegramBotClient)
        {
            _telegramBotClient = telegramBotClient;
        }

        public async Task AnswerInlineQuery(long destanationChatId, string inlineQueryId, List<TgMediaFile> mediaFiles, string? nextOffset = null)
        {
            await _telegramBotClient.AnswerInlineQueryAsync(inlineQueryId,
                mediaFiles.Select(x => ConvertToInlineQueryResultPhoto(x)),
                        0,
                true, nextOffset, "Добавить кружочек ✏️", "add");
        }

        public async Task EmptyAnswerInlineQuery(long destanationChatId, string inlineQueryId)
        {
            await _telegramBotClient.AnswerInlineQueryAsync(inlineQueryId, new List<InlineQueryResultCachedPhoto> { },
                        0,
                true, null, "Добавить кружочек ✏️", "add");
        }

        private static InlineQueryResultCachedVideo ConvertToInlineQueryResultPhoto(TgMediaFile resultVideo)
        {
            var result = new InlineQueryResultCachedVideo(resultVideo.Id.ToString(), resultVideo.FileId, resultVideo.Description ?? resultVideo.Id.ToString());
            return result;
        }

        public async Task SendTextMessage(string text, long destanationChatId, List<string>? textButtons = null)
        {
            ReplyKeyboardMarkup? replyKeyboardMarkup = null;

            if (textButtons != null)
            {
                List<KeyboardButton> keyboardButtons = new List<KeyboardButton>();
                replyKeyboardMarkup = new ReplyKeyboardMarkup(keyboardButtons);

                foreach (var button in textButtons)
                {
                    keyboardButtons.Add(new KeyboardButton(button));
                }

                replyKeyboardMarkup.ResizeKeyboard = true;
                replyKeyboardMarkup.OneTimeKeyboard = true;
            }

            await _telegramBotClient.SendTextMessageAsync(destanationChatId, text, replyMarkup: replyKeyboardMarkup, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }

        public async Task SendTextMessage(string text, long destanationChatId,
            KeyValuePair<string, string>? inlineQueryData = null,
            Dictionary<string, string>? inlineButton = null)
        {
            InlineKeyboardMarkup? inlineKeyboardMarkup = null;
            List<List<InlineKeyboardButton>> buttons = new();

            if (inlineQueryData != null)
            {
                List<InlineKeyboardButton> lineButtons = new();
                lineButtons.Add(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(inlineQueryData.Value.Key, " " + inlineQueryData.Value.Value));
                buttons.Add(lineButtons);
            }

            if (inlineButton != null)
            {
                foreach (var item in ConvertToInlineKeyboardMarkup(inlineButton))
                {
                    List<InlineKeyboardButton> lineButtons = new List<InlineKeyboardButton> { item };
                    buttons.Add(lineButtons);
                }
            }

            if (buttons.Count > 0)
            {
                inlineKeyboardMarkup = new InlineKeyboardMarkup(buttons);
            }

            await _telegramBotClient.SendTextMessageAsync(destanationChatId, text, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: inlineKeyboardMarkup);
        }

        public async Task AnswerCallbackQuery(string callBackQueryId)
        {
            try
            {
                await _telegramBotClient.AnswerCallbackQueryAsync(callBackQueryId);
            }
            catch
            {
            }
        }

        public async Task SendVideo(string fileId, long destanationChatId, string? caption = null, KeyValuePair<string, string>? inlineQueryData = null)
        {
            List<List<InlineKeyboardButton>> buttons = new();

            if (inlineQueryData != null)
            {
                List<InlineKeyboardButton> lineButtons = new();
                lineButtons.Add(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(inlineQueryData.Value.Key, " " + inlineQueryData.Value.Value));
                buttons.Add(lineButtons);
            }

            InlineKeyboardMarkup inlineKeyboardMarkup = new(buttons);

            await _telegramBotClient.SendVideoAsync(destanationChatId,
                new Telegram.Bot.Types.InputFiles.InputOnlineFile(fileId), replyMarkup: inlineKeyboardMarkup,
                caption: caption);
        }

        public async Task<MemoryStream> GetFile(string fileId, MemoryStream destination)
        {
            await _telegramBotClient.GetInfoAndDownloadFileAsync(fileId, destination);

            return destination;
        }

        public async Task<TelegramFileInfoModel> SendVideoNote(MemoryStream memoryStream, long destinationChatId)
        {
            var message = await _telegramBotClient.SendVideoNoteAsync(destinationChatId,
                new Telegram.Bot.Types.InputFiles.InputTelegramFile(memoryStream));

            return new TelegramFileInfoModel
            {
                FileId = message.VideoNote.FileId,
                FileSize = message.VideoNote.FileSize,
                FileType = Domain.Enums.TelegramFileTypeEnum.VideoNote,
                FileUniqueId = message.VideoNote.FileUniqueId,
                VideoDuration = message.VideoNote.Duration
            };
        }

        private static List<InlineKeyboardButton> ConvertToInlineKeyboardMarkup(Dictionary<string, string> inlineButton)
        {
            if (inlineButton == null || inlineButton.Count == 0) return null;

            var buttons = new List<InlineKeyboardButton>();
            foreach (var pair in inlineButton)
            {
                InlineKeyboardButton btn = new(pair.Key)
                {
                    CallbackData = pair.Value
                };

                buttons.Add(btn);
            }

            return buttons;
        }
    }
}