using Domain.Abstractions;
using Domain.Entities;
using System.Collections.Concurrent;

namespace Domain.Services
{
    public class DataStoreProxy : IDataStore
    {
        private readonly IDataStore _dataStore;

        private static readonly ConcurrentDictionary<long, TgUser> UsersDict = new();
        private static readonly ConcurrentDictionary<long, List<TgMediaFile>> MediaFilesDict = new();
        private static readonly ConcurrentDictionary<long, InlineResultStatistics> InlineResultsDict = new();

        public DataStoreProxy(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<TgMediaFile> AddTgMediaFile(TgMediaFile tgMediaFile)
        {
            await _dataStore.AddTgMediaFile(tgMediaFile);

            if (!MediaFilesDict.ContainsKey(tgMediaFile.OwnerTgUserId))
            {
                MediaFilesDict.TryAdd(tgMediaFile.OwnerTgUserId, new List<TgMediaFile> { tgMediaFile });
            }
            else
            {
                MediaFilesDict[tgMediaFile.OwnerTgUserId].Add(tgMediaFile);
            }

            return tgMediaFile;
        }

        public async Task<TgUser> AddTgUser(TgUser tgUser)
        {
            if (UsersDict.ContainsKey(tgUser.ChatId))
            {
                return UsersDict[tgUser.ChatId];
            }

            await _dataStore.AddTgUser(tgUser);

            UsersDict.GetOrAdd(tgUser.ChatId, tgUser);

            return tgUser;
        }

        public async Task<List<TgMediaFile>> GetTgMediaFiles(long ownerUserId)
        {
            if (MediaFilesDict.ContainsKey(ownerUserId))
            {
                return MediaFilesDict[ownerUserId];
            }

            var list = await _dataStore.GetTgMediaFiles(ownerUserId);
            MediaFilesDict.TryAdd(ownerUserId, list);
            return list;
        }

        public async Task<TgUser?> GetTgUser(long chatId)
        {
            if (UsersDict.ContainsKey(chatId))
            {
                return UsersDict[chatId];
            }

            var user = await _dataStore.GetTgUser(chatId);

            if (user != null)
            {
                UsersDict.TryAdd(chatId, user);
            }

            return user;
        }

        public async Task UpdateTgMediaFile(TgMediaFile tgMediaFile)
        {
            await _dataStore.UpdateTgMediaFile(tgMediaFile);
        }

        public async Task<TgMediaFile?> GetTgMediaFile(long chatId, string fileUniqueId)
        {
            var list = await GetTgMediaFiles(chatId);

            return list.Where(x => x.FileUniqueId == fileUniqueId && !x.IsDeleted).FirstOrDefault();
        }

        public async Task<InlineResultStatistics?> GetInlineResultStatistics(long chatId, long tgMediaFileId)
        {
            if (InlineResultsDict.ContainsKey(tgMediaFileId))
            {
                return InlineResultsDict[tgMediaFileId];
            }

            var result = await _dataStore.GetInlineResultStatistics(chatId, tgMediaFileId);

            if (result != null)
            {
                InlineResultsDict.TryAdd(tgMediaFileId, result);
            }

            return result;
        }

        public async Task UpdateInlineResultStatistics(InlineResultStatistics stat)
        {
            await _dataStore.UpdateInlineResultStatistics(stat);
        }

        public async Task SaveUserAction(UserAction userAction)
        {
            await _dataStore.SaveUserAction(userAction);
        }
    }
}