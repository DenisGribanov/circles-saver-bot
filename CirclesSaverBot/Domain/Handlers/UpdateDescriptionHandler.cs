using Domain.Abstractions;
using Domain.Constants;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers
{
    public class UpdateDescriptionHandler : BaseHandler
    {
        private IManagmentStateService _managmentStateService;

        public UpdateDescriptionHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient,
            IManagmentStateService managmentStateService)
            : base(usersStateService, dataStore, telegramClient)
        {
            _managmentStateService = managmentStateService;
        }

        protected override Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            var mngmntState = _managmentStateService.GetState(_messageModel.UserFromId);

            var match = mngmntState != null && mngmntState.Action == ManagmentActionEnum.EDIT_DESCRIPTION &&
                !string.IsNullOrEmpty(_messageModel.MessageText);

            return Task.FromResult(match);
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            if (_messageModel.MessageText.Length > Variables.DescriptionMaxLen)
            {
                await _telegramClient.SendTextMessage($"Максимальная длина {Variables.DescriptionMaxLen} символов ⛔️⛔️⛔️", _messageModel.UserFromId);
                return null;
            }

            var mngmntState = _managmentStateService.GetState(_messageModel.UserFromId);

            var tgFiles = await _dataStore.GetTgMediaFiles(_messageModel.UserFromId);

            var tgMediaFile = tgFiles.FirstOrDefault(x => x.Id == mngmntState.TgMediaFileId);

            if (tgMediaFile == null || tgMediaFile.IsDeleted)
            {
                await _telegramClient.SendTextMessage("Кружочек не найден. Возможно был удален ранее 🤷🏻‍♂️", _messageModel.UserFromId);
                return null;
            }

            tgMediaFile.ModifyDate = DateTime.UtcNow;
            tgMediaFile.Description = _messageModel.MessageText;
            await _dataStore.UpdateTgMediaFile(tgMediaFile);

            _managmentStateService.ClearState(_messageModel.UserFromId);

            var inlineQuery = new KeyValuePair<string, string>("Посмотреть 👀", tgMediaFile.Number.ToString());

            await _telegramClient.SendTextMessage("Описание сохранено ✅", _messageModel.UserFromId, inlineQuery);

            return GetHandlerStateName();
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.DESCRIPTION_UPDATE;
        }
    }
}