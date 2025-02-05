using Domain.Abstractions;
using Moq;

namespace Tests.Services.Handlers
{
    public class InlineQueryHandlerTests
    {
        private readonly Mock<IUsersStateService> stateService = new();
        private readonly Mock<IDataStore> dataStore = new();
        private readonly Mock<ITelegramClient> tgClient = new();

        [Fact]
        public void Match_Is_True()
        {
        }

        [Fact]
        public void Match_Is_False()
        {
        }
    }
}