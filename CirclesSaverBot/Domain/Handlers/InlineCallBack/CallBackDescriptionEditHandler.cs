using Domain.Abstractions;
using Domain.Constants;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers.InlineCallBack
{
    public class CallBackDescriptionEditHandler : ManagmentCallBackBaseHandler
    {
        protected override ManagmentActionEnum ManagmentAction => ManagmentActionEnum.EDIT_DESCRIPTION;

        public CallBackDescriptionEditHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient,
            IManagmentStateService pictureManagmenStateService)
            : base(usersStateService, dataStore, telegramClient, pictureManagmenStateService)
        {
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            var keyboadCallBackData = InlineKeyboadData<InlineManagmentMode>.FromJson(_messageModel.CallBackData);
            var pictures = await _dataStore.GetTgMediaFiles(_messageModel.UserFromId);
            var picture = pictures.FirstOrDefault(x => x.Id == keyboadCallBackData.Data.TgMediaFileId);

            if (picture == null)
            {
                return null;
            }

            string responseText = $"Добавьте описание для поиска \r\n\r\n_Пришлите сообщение не более {Variables.DescriptionMaxLen} символов_ ⚠️";

            _managmentStateService.AddState(_messageModel.UserFromId, picture.Id, ManagmentAction);

            await _telegramClient.SendTextMessage(responseText, _messageModel.UserFromId);

            await _telegramClient.AnswerCallbackQuery(_messageModel.CallBackQueryId);

            return UpdateStateForCurrentUser(_messageModel.UserFromId);
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.CALL_BACK_DESCRIPTION_EDIT;
        }
    }
}