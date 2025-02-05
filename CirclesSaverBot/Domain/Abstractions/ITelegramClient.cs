using Domain.Entities;
using Domain.Models.Telegram;

namespace Domain.Abstractions
{
    public interface ITelegramClient
    {
        Task EmptyAnswerInlineQuery(long destanationChatId, string inlineQueryId);

        Task AnswerInlineQuery(long destanationChatId, string inlineQueryId, List<TgMediaFile> pictures, string? nextOffset = null);

        Task SendTextMessage(string text, long destanationChatId, List<string>? textButtons = null);

        Task SendTextMessage(string text, long destanationChatId, KeyValuePair<string, string>? inlineQueryData, Dictionary<string, string>? inlineButton = null);

        Task AnswerCallbackQuery(string callBackQueryId);

        Task SendVideo(string fileId, long destanationChatId, string? caption = null, KeyValuePair<string, string>? inlineQueryData = null);

        Task<MemoryStream> GetFile(string fileId, MemoryStream destination);

        Task<TelegramFileInfoModel> SendVideoNote(MemoryStream memoryStream, long destinationChatId);
    }
}