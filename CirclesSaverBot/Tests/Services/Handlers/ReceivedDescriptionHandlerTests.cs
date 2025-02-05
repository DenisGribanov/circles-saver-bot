using Domain.Abstractions;
using Domain.Enums;
using Domain.Handlers;
using Domain.Models.Telegram;
using Moq;

namespace Tests.Services.Handlers
{
    public class ReceivedDescriptionHandlerTests
    {
        private readonly Mock<IUsersStateService> stateService = new();
        private readonly Mock<IDataStore> dataStore = new();
        private readonly Mock<ITelegramClient> tgClient = new();

        [Fact]
        public async void Match_Is_True()
        {
            const int userId = 1;
            TelegramMessageModel telegramMessage = new TelegramMessageModel
            {
                UserFromId = userId,
                Username = "user",
                MessageText = "Text"
            };

            stateService.Setup(x => x.GetStateNow(userId)).Returns(() =>
            {
                return UserStateTypeEnum.VIDEO_NOTE_UPLOADED;
            });

            DescriptionFromNewFileHandler handler = new DescriptionFromNewFileHandler(stateService.Object, dataStore.Object, tgClient.Object);

            await handler.HandleAsync(telegramMessage);

            Assert.True(handler.IsMatchForTelegramUpdate);
        }

        [Fact]
        public async void Match_Is_False()
        {
            const int userId = 1;
            TelegramMessageModel telegramMessage = new TelegramMessageModel
            {
                UserFromId = userId,
                Username = "user",
                MessageText = "Text"
            };

            stateService.Setup(x => x.GetStateNow(userId)).Returns(() =>
            {
                return null;
            });

            DescriptionFromNewFileHandler handler = new DescriptionFromNewFileHandler(stateService.Object, dataStore.Object, tgClient.Object);

            await handler.HandleAsync(telegramMessage);

            Assert.False(handler.IsMatchForTelegramUpdate);
        }

        [Fact]
        public async void Return_Is_Caption_Added()
        {
            const int userId = 1;

            TelegramMessageModel telegramMessage = new TelegramMessageModel
            {
                UserFromId = userId,
                Username = "user",
                MessageText = "Text"
            };

            stateService.Setup(x => x.GetStateNow(userId)).Returns(() =>
            {
                return UserStateTypeEnum.VIDEO_NOTE_UPLOADED;
            });

            dataStore.Setup(x => x.GetTgMediaFiles(userId)).Returns(async () =>
            {
                await Task.Delay(1);

                return new List<Domain.Entities.TgMediaFile>
                {
                     new Domain.Entities.TgMediaFile
                     {
                          IsVisable = false,
                           OwnerTgUserId = userId
                     }
                };
            });

            DescriptionFromNewFileHandler handler = new DescriptionFromNewFileHandler(stateService.Object, dataStore.Object, tgClient.Object);

            var result = await handler.HandleAsync(telegramMessage);

            Assert.Equal(UserStateTypeEnum.VIDEO_NOTE_DESCRIPTION_ADDED, result);
        }

        [Fact]
        public async void Return_Is_Null()
        {
            const int userId = 1;

            TelegramMessageModel telegramMessage = new TelegramMessageModel
            {
                UserFromId = userId,
                Username = "user",
                MessageText = "Text"
            };

            stateService.Setup(x => x.GetStateNow(userId)).Returns(() =>
            {
                return UserStateTypeEnum.VIDEO_NOTE_UPLOADED;
            });

            dataStore.Setup(x => x.GetTgMediaFiles(userId)).Returns(async () =>
            {
                await Task.Delay(1);

                return new List<Domain.Entities.TgMediaFile>
                {
                     new Domain.Entities.TgMediaFile
                     {
                          IsVisable = true,
                          OwnerTgUserId = userId,
                          Description = "Text"
                     }
                };
            });

            DescriptionFromNewFileHandler handler = new DescriptionFromNewFileHandler(stateService.Object, dataStore.Object, tgClient.Object);

            var result = await handler.HandleAsync(telegramMessage);

            Assert.Null(result);
        }
    }
}