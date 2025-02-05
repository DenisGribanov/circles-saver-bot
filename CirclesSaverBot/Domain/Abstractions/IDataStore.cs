using Domain.Entities;

namespace Domain.Abstractions
{
    public interface IDataStore
    {
        Task<TgUser?> GetTgUser(long chatId);

        Task<TgUser> AddTgUser(TgUser tgUser);

        Task<TgMediaFile> AddTgMediaFile(TgMediaFile tgMediaFile);

        Task<List<TgMediaFile>> GetTgMediaFiles(long chatId);

        Task UpdateTgMediaFile(TgMediaFile tgMediaFile);

        Task<TgMediaFile?> GetTgMediaFile(long chatId, string fileUniqueId);

        Task<InlineResultStatistics?> GetInlineResultStatistics(long chatId, long tgMediaFileId);

        Task UpdateInlineResultStatistics(InlineResultStatistics stat);

        Task SaveUserAction(UserAction userAction);
    }
}