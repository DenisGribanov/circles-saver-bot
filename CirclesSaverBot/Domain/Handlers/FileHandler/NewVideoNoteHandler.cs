using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Telegram;
using Microsoft.Extensions.Logging;

namespace Domain.Handlers.FileHandler
{
    public class NewVideoNoteHandler : BaseHandler
    {
        private readonly IVideoStickersBotClient _videoStickersBotClient;

        public NewVideoNoteHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient,
            IVideoStickersBotClient videoStickersBotClient) : base(usersStateService, dataStore, telegramClient)
        {
            _videoStickersBotClient = videoStickersBotClient;
        }

        protected override async Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            if (_messageModel.TelegramFile == null)
            {
                return false;
            }

            var tgMediaFile = await _dataStore.GetTgMediaFile(_messageModel.UserFromId, _messageModel.TelegramFile.FileUniqueId);

            var match = tgMediaFile == null && _messageModel.TelegramFile.FileType != null
                && _messageModel.TelegramFile.FileType == TelegramFileTypeEnum.VideoNote;

            return match;
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            var stickerInfo = await GetVideoStickersInfo(_messageModel.TelegramFile.FileUniqueId);
            string? description = stickerInfo != null ? string.Format("{0} - {1}", stickerInfo.HashTags, stickerInfo.Description) : null;

            var tgMediaFile = await _dataStore.AddTgMediaFile(new TgMediaFile
            {
                CreateDate = DateTime.UtcNow,
                FileId = _messageModel.TelegramFile.FileId,
                FileSize = _messageModel.TelegramFile.FileSize ?? default,
                FileUniqueId = _messageModel.TelegramFile.FileUniqueId,
                VideoDuration = _messageModel.TelegramFile.VideoDuration,
                OwnerTgUserId = _messageModel.UserFromId,
                Description = description,
                IsVisable = !string.IsNullOrEmpty(description)
            });

            string responseForDescriptionNotExist = "Кружочек добавлен ✅.\r\n\r\nДобавьте описание для поиска (пришлите сообщение не более 50 символов)";
            string responseForDescriptionExist = $"Кружочек добавлен ✅\r\n\r\n_{description}_";

            if (description == null)
            {
                await _telegramClient.SendTextMessage(responseForDescriptionNotExist, _messageModel.UserFromId, new List<string> { "Без описания" });
            }
            else
            {
                await _telegramClient.SendTextMessage(responseForDescriptionExist, _messageModel.UserFromId, new KeyValuePair<string, string>("Посмотреть 👀", tgMediaFile.Number.ToString()));
            }

            return UpdateStateForCurrentUser(_messageModel.UserFromId);
        }

        private async Task<VideoStickersInfoResponse?> GetVideoStickersInfo(string fileUniqueId)
        {
            try
            {
                _logger.LogInformation("Запрос к VideoStickersBot");

                return await _videoStickersBotClient.GetStickerDescription(fileUniqueId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запросе к VideoStickersBot");
                return null;
            }
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.VIDEO_NOTE_UPLOADED;
        }
    }
}