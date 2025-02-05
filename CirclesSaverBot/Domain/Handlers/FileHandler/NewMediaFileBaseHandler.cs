using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models.Telegram;
using Domain.Options;

namespace Domain.Handlers.FileHandler
{
    public abstract class NewMediaFileBaseHandler : BaseHandler
    {
        private readonly IVideoResize _videoResize;

        public NewMediaFileBaseHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient,
            IVideoResize videoResize)
            : base(usersStateService, dataStore, telegramClient)
        {
            _videoResize = videoResize;
        }

        protected void Validate(TelegramFileInfoModel fileInfoModel)
        {
            if (fileInfoModel.VideoDuration > EnvironmentOptionsHelper.EnvironmentOptions.MaxVideoDuration)
            {
                throw new ValidateException($"Ошибка ⚠️ Максимальная продолжительность видео {EnvironmentOptionsHelper.EnvironmentOptions.MaxVideoDuration} сек.");
            }

            if (fileInfoModel.FileSize.HasValue && fileInfoModel.FileSize > EnvironmentOptionsHelper.EnvironmentOptions.MaxFileSize)
            {
                throw new ValidateException($"Ошибка ⚠️ Максимальный размер видео 10 мб.");
            }
        }

        protected async Task<TgMediaFile> ConvertToVideoNote(string fileId, long userId)
        {
            using MemoryStream sourceVideo = new();

            await _telegramClient.GetFile(fileId, sourceVideo);

            await _telegramClient.SendTextMessage("Идет обработка 🔄. Пожалуйста подождите 🙏", userId);

            var squareVideo = await _videoResize.ConvertToSquareAsync(sourceVideo);

            await SaveFile(squareVideo, fileId + ".mp4");

            var message = await _telegramClient.SendVideoNote(new MemoryStream(squareVideo), userId);

            return await SaveTgMediaFile(message, userId);
        }

        private async Task SaveFile(byte[] bytes, string fileName, string path = null)
        {
            try
            {
                if (bytes == null) throw new ArgumentNullException("bytes");

                string pathSave = path == null ? fileName : Path.Combine(path, fileName);
                await System.IO.File.WriteAllBytesAsync(pathSave, bytes);
            }
            catch (Exception ex)
            {
            }
        }

        private async Task<TgMediaFile> SaveTgMediaFile(TelegramFileInfoModel telegramFileInfo, long ownerUserId)
        {
            return await _dataStore.AddTgMediaFile(new TgMediaFile
            {
                CreateDate = DateTime.UtcNow,
                FileId = telegramFileInfo.FileId,
                FileSize = telegramFileInfo.FileSize ?? default,
                FileUniqueId = telegramFileInfo.FileUniqueId,
                VideoDuration = telegramFileInfo.VideoDuration,
                OwnerTgUserId = ownerUserId
            });
        }
    }
}