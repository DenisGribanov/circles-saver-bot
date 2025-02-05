using Domain.Abstractions;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers
{
    public class DescriptionFromNewFileHandler : BaseHandler
    {
        public DescriptionFromNewFileHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient) : base(usersStateService, dataStore, telegramClient)
        {
        }

        protected override Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            var currentState = _usersStateService.GetStateNow(_messageModel.UserFromId);
            var states = new List<UserStateTypeEnum>
            {
                UserStateTypeEnum.VIDEO_NOTE_UPLOADED,
                UserStateTypeEnum.VIDEO_FILE_UPLOADED,
                UserStateTypeEnum.DOC_FILE_UPLOADED
            };

            bool match = currentState.HasValue && states.Contains(currentState.Value) && !string.IsNullOrEmpty(_messageModel.MessageText);

            return Task.FromResult(match);
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            if (_messageModel.MessageText.Length > 50)
            {
                await _telegramClient.SendTextMessage("Максимум 50 символов ⚠️⚠️⚠️", _messageModel.UserFromId);
                return null;
            }

            var mediaFiles = await _dataStore.GetTgMediaFiles(_messageModel.UserFromId);

            var lastPic = mediaFiles.Where(x => !x.IsDeleted && !x.IsVisable && string.IsNullOrEmpty(x.Description)).LastOrDefault();

            if (lastPic == null) return null;

            lastPic.Description = _messageModel.MessageText.Trim();
            lastPic.IsVisable = true;
            lastPic.ModifyDate = DateTime.UtcNow;

            await _dataStore.UpdateTgMediaFile(lastPic);

            await SendTextMessage(lastPic.Number, _messageModel.UserFromId);

            return UpdateStateForCurrentUser(_messageModel.UserFromId);
        }

        private async Task SendTextMessage(long number, long userId)
        {
            var inlineQuery = new KeyValuePair<string, string>("Посмотреть 👀", number.ToString());

            await _telegramClient.SendTextMessage("Описание сохранено 💾", userId, inlineQuery);
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.VIDEO_NOTE_DESCRIPTION_ADDED;
        }
    }
}