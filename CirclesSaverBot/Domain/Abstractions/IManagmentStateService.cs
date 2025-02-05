using Domain.Enums;
using Domain.Models;

namespace Domain.Abstractions
{
    public interface IManagmentStateService
    {
        ManagmentModeState? GetState(long userChatId);

        ManagmentModeState AddState(long userChatId, long pictureId, ManagmentActionEnum action);

        bool ClearState(long userChatId);
    }
}