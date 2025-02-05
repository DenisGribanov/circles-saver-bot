using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Abstractions
{
    public interface ITelegramMessageHandler
    {
        Task<UserStateTypeEnum?> HandleAsync(TelegramMessageModel _messageModel);
    }
}