using Domain.Enums;
using Domain.Services;

namespace Tests.Services
{
    public class UsersStateServiceTest
    {
        private UsersStateService usersStateService = new UsersStateService();

        [Fact]
        public void Update_State_Is_Not_Null()
        {
            const int userId = 188113;
            const UserStateTypeEnum botState = UserStateTypeEnum.VIDEO_NOTE_DESCRIPTION_ADDED;

            usersStateService.UpdateState(userId, botState);

            var stateNow = usersStateService.GetStateNow(userId);

            Assert.Equal(stateNow, botState);
        }

        [Fact]
        public void User_State_Is_Null()
        {
            const int userId = 22213;

            var stateNow = usersStateService.GetStateNow(userId);

            Assert.Null(stateNow);
        }

        [Fact]
        public void User_Previous_Is_Not_Null()
        {
            const int userId = 33321;
            const UserStateTypeEnum botStatePhotoUploaded = UserStateTypeEnum.VIDEO_NOTE_UPLOADED;

            usersStateService.UpdateState(userId, botStatePhotoUploaded);

            usersStateService.UpdateState(userId, UserStateTypeEnum.VIDEO_NOTE_DESCRIPTION_ADDED);

            var previous = usersStateService.GetStatePrevious(userId);

            Assert.Equal(botStatePhotoUploaded, previous);
        }

        [Fact]
        public void User_Previous_Is_Null()
        {
            const int userId = 433221;

            usersStateService.UpdateState(userId, UserStateTypeEnum.VIDEO_NOTE_DESCRIPTION_ADDED);

            var previous = usersStateService.GetStatePrevious(userId);

            Assert.Null(previous);
        }
    }
}