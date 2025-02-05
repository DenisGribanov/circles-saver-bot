using Domain.Abstractions;
using Domain.Enums;
using Domain.Models;
using System.Collections.Concurrent;

namespace Domain.Services
{
    public class ManagmenStateService : IManagmentStateService
    {
        private static readonly ConcurrentDictionary<long, ManagmentModeState> States = new ConcurrentDictionary<long, ManagmentModeState>();

        public ManagmentModeState? GetState(long userChatId)
        {
            return States.GetValueOrDefault(userChatId);
        }

        public ManagmentModeState AddState(long userChatId, long tgMediaFileId, ManagmentActionEnum action)
        {
            var state = new ManagmentModeState
            {
                TgMediaFileId = tgMediaFileId,
                Action = action
            };

            return States.AddOrUpdate(userChatId, state, (key, oldVal) => state);
        }

        public bool ClearState(long userChatId)
        {
            return States.TryRemove(userChatId, out _);
        }
    }
}