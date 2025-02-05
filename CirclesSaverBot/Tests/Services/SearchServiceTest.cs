using Domain.Abstractions;
using Domain.Entities;
using Domain.Services;
using Moq;

namespace Tests.Services
{
    public class SearchServiceTest
    {
        private readonly Mock<IDataStore> dataStore = new Mock<IDataStore>();

        [Fact]
        public async void Return_Next_Offset_Greater_Then_Zero()
        {
            const int userId = 1;
            const int maxResult = 5;
            const int elAmount = 17;
            const int nextOffsetResultExpected = 1;
            const int totalPageResultExpected = 4;

            SearchService searchPicturesService = new SearchService(dataStore.Object);

            dataStore.Setup(x => x.GetTgMediaFiles(userId)).Returns(async () =>
            {
                await Task.Delay(1);
                return GetPicturesData(userId, elAmount);
            });

            var result = await searchPicturesService.Search(userId, maxResult);

            Assert.Equal(nextOffsetResultExpected, result.NextOffset);
            Assert.Equal(totalPageResultExpected, result.TotalPage);
        }

        [Fact]
        public async void If_Offet_Is_One_Return_Next_Offset_Equal_Two()
        {
            const int userId = 1;
            const int maxResult = 5;
            const int offset = 1;
            const int elAmount = 17;
            const int offsetResultExpected = 2;

            SearchService searchPicturesService = new SearchService(dataStore.Object);

            dataStore.Setup(x => x.GetTgMediaFiles(userId)).Returns(async () =>
            {
                await Task.Delay(1);
                return GetPicturesData(userId, elAmount);
            });

            var result = await searchPicturesService.Search(userId, maxResult, null, offset);

            Assert.Equal(offsetResultExpected, result.NextOffset.Value);
        }

        [Fact]
        public async void If_Offset_Is_Last_Return_Next_Offset_Equal_Null()
        {
            const int userId = 1;
            const int maxResult = 5;
            const int offset = 4;
            const int elAmount = 17;
            const int picCountResultExpected = 2;

            SearchService searchPicturesService = new SearchService(dataStore.Object);

            dataStore.Setup(x => x.GetTgMediaFiles(userId)).Returns(async () =>
            {
                await Task.Delay(1);
                return GetPicturesData(userId, elAmount);
            });

            var result = await searchPicturesService.Search(userId, maxResult, null, offset);

            Assert.Null(result.NextOffset);
            Assert.Equal(picCountResultExpected, result.Pictures.Count);
        }

        [Fact]
        public async void Return_Offset_Is_Null()
        {
            const int userId = 1;
            const int maxResult = 100;
            SearchService searchPicturesService = new SearchService(dataStore.Object);

            dataStore.Setup(x => x.GetTgMediaFiles(userId)).Returns(async () =>
            {
                await Task.Delay(1);
                return GetPicturesData(userId);
            });

            var result = await searchPicturesService.Search(userId, maxResult);

            Assert.Null(result.NextOffset);
        }

        [Fact]
        public async void If_Offset_Is_Not_Exist_Then_Return_Empty()
        {
            const int userId = 1;
            const int maxResult = 5;
            const int offset = 100;
            SearchService searchPicturesService = new SearchService(dataStore.Object);

            dataStore.Setup(x => x.GetTgMediaFiles(userId)).Returns(async () =>
            {
                await Task.Delay(1);
                return GetPicturesData(userId);
            });

            var result = await searchPicturesService.Search(userId, maxResult, null, offset);

            Assert.Empty(result.Pictures);
        }

        private static List<TgMediaFile> GetPicturesData(long userId, int count = 10)
        {
            List<TgMediaFile> result = new List<TgMediaFile>();
            for (int i = 0; i < count; i++)
            {
                var pic = new TgMediaFile { OwnerTgUserId = userId, IsVisable = true };
                result.Add(pic);
            }

            return result;
        }
    }
}