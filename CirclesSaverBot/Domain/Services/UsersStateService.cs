using Domain.Abstractions;
using Domain.Enums;
using Domain.Models;
using System.Collections.Concurrent;

namespace Domain.Services
{
    public class UsersStateService : IUsersStateService
    {
        private static readonly ConcurrentDictionary<long, UserState> usersState = new();

        public UserStateTypeEnum? GetStateNow(long userId)
        {
            if (usersState.ContainsKey(userId))
            {
                var state = usersState[userId];

                return state.StateNow;
            }

            return null;
        }

        public UserStateTypeEnum? GetStatePrevious(long userId)
        {
            if (usersState.ContainsKey(userId))
            {
                var state = usersState[userId];

                return state.StatePrevious;
            }

            return null;
        }

        public void UpdateState(long userId, UserStateTypeEnum userStateType)
        {
            if (!usersState.ContainsKey(userId))
            {
                usersState.TryAdd(userId, new UserState
                {
                    StateNow = userStateType,
                });

                return;
            }

            var state = usersState[userId];
            state.StatePrevious = state.StateNow;
            state.StateNow = userStateType;
        }
    }
}