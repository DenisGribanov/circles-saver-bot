using Domain.Abstractions;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers
{
    public class ConfirmRemoveHandler : BaseHandler
    {
        private readonly IManagmentStateService _managmentStateService;

        public ConfirmRemoveHandler(IUsersStateService usersStateService,
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

            if (mngmntState == null || mngmntState.Action != ManagmentActionEnum.REMOVE)
            {
                return Task.FromResult(false);
            }
            else
            {
                return Task.FromResult(Constants.RemoveMessage.RemoveConfirm.Equals(_messageModel.MessageText, StringComparison.OrdinalIgnoreCase));
            }
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            var mngmntState = _managmentStateService.GetState(_messageModel.UserFromId);

            var pictures = await _dataStore.GetTgMediaFiles(_messageModel.UserFromId);

            var picture = pictures.FirstOrDefault(x => x.Id == mngmntState.TgMediaFileId);

            if (picture == null || picture.IsDeleted)
            {
                await _telegramClient.SendTextMessage("Кружочек не найден. Возможно был удален ранее 🤷🏻‍♂️", _messageModel.UserFromId);
                return null;
            }

            picture.DeletedDate = DateTime.UtcNow;
            picture.IsDeleted = true;

            await _dataStore.UpdateTgMediaFile(picture);

            _managmentStateService.ClearState(_messageModel.UserFromId);

            var inlineQuery = new KeyValuePair<string, string>("Мои кружлочки 🔵", string.Empty);

            await _telegramClient.SendTextMessage("Кружочек удален!", _messageModel.UserFromId, inlineQuery);

            return GetHandlerStateName();
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.CONFIRM_REMOVE;
        }
    }
}