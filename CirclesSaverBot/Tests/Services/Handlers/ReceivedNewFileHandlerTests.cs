using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Handlers.FileHandler;
using Domain.Models.Telegram;
using Moq;

namespace Tests.Services.Handlers
{
    public class ReceivedNewFileHandlerTests
    {
        private readonly Mock<IUsersStateService> stateService = new();
        private readonly Mock<IDataStore> dataStore = new();
        private readonly Mock<ITelegramClient> tgClient = new();
        private readonly Mock<IVideoStickersBotClient> videoStickersBotClient = new();

        [Fact]
        public async void Match_Is_True()
        {
            TelegramMessageModel telegramMessage = new()
            {
                UserFromId = 1,
                Username = "user",
                TelegramFile = new TelegramFileInfoModel
                {
                    FileType = TelegramFileTypeEnum.VideoNote,
                    FileId = "1111",
                    FileSize = 1000,
                    FileUniqueId = "222",
                }
            };

            NewVideoNoteHandler receivedPhotoHandler = new NewVideoNoteHandler(stateService.Object, dataStore.Object, tgClient.Object, videoStickersBotClient.Object);

            await receivedPhotoHandler.HandleAsync(telegramMessage);

            Assert.True(receivedPhotoHandler.IsMatchForTelegramUpdate);
        }

        [Fact]
        public async void Match_Is_False()
        {
            TelegramMessageModel telegramMessage = new TelegramMessageModel
            {
                UserFromId = 1,
                Username = "user",
            };

            NewVideoNoteHandler receivedPhotoHandler = new NewVideoNoteHandler(stateService.Object, dataStore.Object, tgClient.Object, videoStickersBotClient.Object);

            await receivedPhotoHandler.HandleAsync(telegramMessage);

            Assert.False(receivedPhotoHandler.IsMatchForTelegramUpdate);
        }

        [Fact]
        public async void UserState_Is_Photo_Uploaded()
        {
            TelegramMessageModel telegramMessage = new()
            {
                UserFromId = 1,
                Username = "user",
                TelegramFile = new TelegramFileInfoModel
                {
                    FileType = TelegramFileTypeEnum.VideoNote,
                    FileId = "1111",
                    FileSize = 1000,
                    FileUniqueId = "222",
                }
            };

            NewVideoNoteHandler receivedPhotoHandler = new NewVideoNoteHandler(stateService.Object, dataStore.Object, tgClient.Object, videoStickersBotClient.Object);

            Assert.Equal(UserStateTypeEnum.VIDEO_NOTE_UPLOADED, await receivedPhotoHandler.HandleAsync(telegramMessage));
        }

        [Fact]
        public async void Photo_Is_Alredy_Exist_Return_Null()
        {
            TelegramMessageModel telegramMessage = new()
            {
                UserFromId = 1,
                Username = "user",
                TelegramFile = new TelegramFileInfoModel
                {
                    FileType = TelegramFileTypeEnum.VideoNote,
                    FileId = "1111",
                    FileSize = 1000,
                    FileUniqueId = "222",
                }
            };

            dataStore.Setup(x => x.GetTgMediaFile(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(() =>
            {
                return new TgMediaFile { };
            });

            NewVideoNoteHandler receivedPhotoHandler = new NewVideoNoteHandler(stateService.Object, dataStore.Object, tgClient.Object, videoStickersBotClient.Object);

            Assert.Null(await receivedPhotoHandler.HandleAsync(telegramMessage));
        }
    }
}