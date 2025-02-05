using Domain.Abstractions;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Services
{
    public class BotService : IBotService
    {
        private readonly IDataStore _dataStoreProxy;
        private readonly IEnumerable<ITelegramMessageHandler> _telegramMessageHandlers;

        public BotService(IDataStore dataStoreProxy,
                            ITelegramClient telegramClient,
                            IUsersStateService usersStateService,
                            IManagmentStateService managmentStateService,
                            IVideoStickersBotClient videoStickersBotClient,
                            IVideoResize videoResize,
                            ISearchService searchService,
                            IEnumerable<ITelegramMessageHandler> telegramMessageHandlers
                            )
        {
            _dataStoreProxy = dataStoreProxy;
            _telegramMessageHandlers = telegramMessageHandlers;
        }

        public async Task<UserStateTypeEnum?> Run(TelegramMessageModel message)
        {
            if (message == null)
            {
                return null;
            }

            await SaveMessage(message);

            await RegUser(message);

            var results = await Task.WhenAll(_telegramMessageHandlers.Select(x => x.HandleAsync(message)));

            return results.Where(x => x is not null).FirstOrDefault();
        }

        private async Task RegUser(TelegramMessageModel telegramMessageModel)
        {
            var user = await _dataStoreProxy.GetTgUser(telegramMessageModel.UserFromId);

            if (user != null) return;

            await _dataStoreProxy.AddTgUser(new Entities.TgUser()
            {
                ChatId = telegramMessageModel.UserFromId,
                UserName = telegramMessageModel.Username,
                CreateDate = DateTime.UtcNow,
            });
        }

        private async Task SaveMessage(TelegramMessageModel telegramMessageModel)
        {
            await _dataStoreProxy.SaveUserAction(new Entities.UserAction
            {
                CreateDate = DateTime.UtcNow,
                TelegramUpdate = telegramMessageModel.SourceUpdateMessageJson,
            });
        }
    }
}