using Domain.Abstractions;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers
{
    public class InlineResultHandler : BaseHandler
    {
        public InlineResultHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient)
            : base(usersStateService, dataStore, telegramClient)
        {
        }

        protected override Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            return Task.FromResult(_messageModel.IsChosenInlineResult);
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            long tgFileId = default;

            if (!long.TryParse(_messageModel.ChosenInlineResultId, out tgFileId))
            {
                return null;
            }

            if (tgFileId < 1) return null;

            var stat = await _dataStore.GetInlineResultStatistics(_messageModel.UserFromId, tgFileId);

            stat ??= new Entities.InlineResultStatistics
            {
                ClickCount = default,
                TgMediaFileId = tgFileId,
                TgUserId = _messageModel.UserFromId
            };

            stat.ClickCount += 1;
            await _dataStore.UpdateInlineResultStatistics(stat);

            return GetHandlerStateName();
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.INLINE_RESULT;
        }
    }
}