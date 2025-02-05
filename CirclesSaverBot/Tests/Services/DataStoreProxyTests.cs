using Domain.Abstractions;
using Domain.Entities;
using Domain.Services;
using Moq;

namespace Tests.Services
{
    public class DataStoreProxyTests
    {
        private readonly Mock<IDataStore> store = new();

        [Fact]
        public async Task GetTgUser()
        {
            const int chatId = 1;

            store.Setup(x => x.GetTgUser(chatId)).Returns(async () =>
            {
                await Task.Delay(1);
                return new TgUser
                {
                    ChatId = chatId,
                };
            });

            var storeProxy = new DataStoreProxy(store.Object);

            var user = await storeProxy.GetTgUser(chatId);

            Assert.NotNull(user);
            Assert.Equal(chatId, user.ChatId);
        }

        [Fact]
        public async Task GetTgUser_Return_Null()
        {
            const int chatId = 1223;

            store.Setup(x => x.GetTgUser(chatId)).Returns(async () =>
            {
                await Task.Delay(1);
                return null;
            });

            var storeProxy = new DataStoreProxy(store.Object);

            var user = await storeProxy.GetTgUser(chatId);

            Assert.Null(user);
        }

        [Fact]
        public async Task Update_Picture()
        {
            const int chatId = 1;
            const int picId = 1;
            const string captionVal = "My pic";

            TgMediaFile addPicture = new()
            {
                Id = picId,
                CreateDate = DateTime.Now,
                FileId = "1",
                FileSize = 1000,
                OwnerTgUserId = chatId
            };

            store.Setup(x => x.GetTgMediaFiles(chatId)).Returns(async () =>
            {
                await Task.Delay(1);
                return new List<TgMediaFile>
                {
                     addPicture
                };
            });

            var storeProxy = new DataStoreProxy(store.Object);
            await storeProxy.AddTgMediaFile(addPicture);

            var picByUser = await storeProxy.GetTgMediaFiles(chatId);
            var updPic = picByUser.First(x => x.Id == picId);

            updPic.Description = captionVal;
            updPic.IsDeleted = true;
            await storeProxy.UpdateTgMediaFile(updPic);

            picByUser = await storeProxy.GetTgMediaFiles(chatId);
            updPic = picByUser.First(x => x.Id == picId);

            Assert.Equal(addPicture, updPic);
            Assert.Equal(captionVal, updPic.Description);
            Assert.True(updPic.IsDeleted);
        }
    }
}