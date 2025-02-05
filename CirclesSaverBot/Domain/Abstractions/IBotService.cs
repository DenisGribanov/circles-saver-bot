using Domain.Enums;

namespace Domain.Abstractions
{
    public interface IBotService
    {
        Task<UserStateTypeEnum?> Run(Models.Telegram.TelegramMessageModel message);
    }
}