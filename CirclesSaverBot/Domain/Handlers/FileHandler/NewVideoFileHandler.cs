using Domain.Abstractions;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers.FileHandler
{
    public class NewVideoFileHandler : NewMediaFileBaseHandler
    {
        public NewVideoFileHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient,
            IVideoResize videoResize)
            : base(usersStateService, dataStore, telegramClient, videoResize)
        {
        }

        protected override Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            return Task.FromResult(TelegramFileTypeEnum.Video == _messageModel.TelegramFile?.FileType);
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            try
            {
                Validate(_messageModel.TelegramFile);

                var mediaFile = await ConvertToVideoNote(_messageModel.TelegramFile.FileId, _messageModel.UserFromId);

                string responseForDescriptionNotExist = "Кружочек создан ✅.\r\n\r\nДобавьте описание для поиска (пришлите сообщение не более 20 символов)";

                await _telegramClient.SendTextMessage(responseForDescriptionNotExist, _messageModel.UserFromId, new List<string> { "Без описания" });

                return UpdateStateForCurrentUser(_messageModel.UserFromId);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("VOICE_MESSAGES_FORBIDDEN", StringComparison.OrdinalIgnoreCase))
                {
                    await _telegramClient.SendTextMessage("Я не могу отправить вам файл 🤷🏻‍♂️ у вас строит запрет на получение видеосообщений. Отключите запрет !", _messageModel.UserFromId);
                    throw;
                }

                await _telegramClient.SendTextMessage("Произошла ошбика при обработке видео 🤷🏻‍♂️", _messageModel.UserFromId);

                throw;
            }
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.VIDEO_FILE_UPLOADED;
        }
    }
}