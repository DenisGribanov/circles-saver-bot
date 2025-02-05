using Domain.Abstractions;
using Domain.Constants;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers.InlineCallBack
{
    public class CallBackRemoveHandler : ManagmentCallBackBaseHandler
    {
        protected override ManagmentActionEnum ManagmentAction => ManagmentActionEnum.REMOVE;

        public CallBackRemoveHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient,
            IManagmentStateService pictureManagmenStateService)
            : base(usersStateService, dataStore, telegramClient, pictureManagmenStateService)
        {
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            var keyboadCallBackData = InlineKeyboadData<InlineManagmentMode>.FromJson(_messageModel.CallBackData);
            var tgMediaFiles = await _dataStore.GetTgMediaFiles(_messageModel.UserFromId);
            var tgMediaFile = tgMediaFiles.FirstOrDefault(x => x.Id == keyboadCallBackData.Data.TgMediaFileId);

            if (tgMediaFile == null)
            {
                return null;
            }

            string responseText = $"id:{tgMediaFile.Number} - удалить кружочек ? 🗑";
            List<string> buttons = new List<string> { RemoveMessage.RemoveConfirm, RemoveMessage.RemoveCancel };

            _managmentStateService.AddState(_messageModel.UserFromId, tgMediaFile.Id, ManagmentAction);

            await _telegramClient.SendTextMessage(responseText, _messageModel.UserFromId, buttons);

            await _telegramClient.AnswerCallbackQuery(_messageModel.CallBackQueryId);

            return UpdateStateForCurrentUser(_messageModel.UserFromId);
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.CALL_BACK_REMOVE;
        }
    }
}