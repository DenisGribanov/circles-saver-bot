using Domain.Enums;

namespace Domain.Abstractions
{
    public interface IUsersStateService
    {
        UserStateTypeEnum? GetStateNow(long userId);

        UserStateTypeEnum? GetStatePrevious(long userId);

        void UpdateState(long userId, UserStateTypeEnum state);
    }
}