using Domain.Enums;

namespace Domain.Models
{
    public class UserState
    {
        public UserStateTypeEnum? StateNow { get; set; }

        public UserStateTypeEnum? StatePrevious { get; set; }
    }
}