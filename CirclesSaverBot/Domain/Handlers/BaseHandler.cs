using Domain.Abstractions;
using Domain.Enums;
using Domain.Models.Telegram;
using Microsoft.Extensions.Logging;

namespace Domain.Handlers
{
    public abstract class BaseHandler : ITelegramMessageHandler
    {
        protected readonly IUsersStateService _usersStateService;

        protected readonly IDataStore _dataStore;

        protected readonly ITelegramClient _telegramClient;

        public bool? IsMatchForTelegramUpdate { get; private set; }

        protected readonly ILogger _logger;

        protected BaseHandler(IUsersStateService usersStateService, IDataStore dataStore, ITelegramClient telegramClient)
        {
            _usersStateService = usersStateService;
            _dataStore = dataStore;
            _telegramClient = telegramClient;
            _logger = LoggerFactory.Create(opt => { }).CreateLogger<BaseHandler>();
        }

        public async Task<UserStateTypeEnum?> HandleAsync(TelegramMessageModel _messageModel)
        {
            if (IsMatchForTelegramUpdate is null)
            {
                IsMatchForTelegramUpdate = await MatchInternal(_messageModel);
            }

            if (IsMatchForTelegramUpdate.GetValueOrDefault())
            {
                return await HandleInternal(_messageModel);
            }
            else
            {
                return null;
            }
        }

        protected abstract Task<bool> MatchInternal(TelegramMessageModel _messageModel);

        protected abstract Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel);

        protected abstract UserStateTypeEnum GetHandlerStateName();

        protected UserStateTypeEnum UpdateStateForCurrentUser(long userId)
        {
            _usersStateService.UpdateState(userId, GetHandlerStateName());

            return GetHandlerStateName();
        }

        protected UserStateTypeEnum? GetUserStateNow(long userId)
        {
            return _usersStateService.GetStateNow(userId);
        }

        protected static InlineKeyboardTypeEnum GetKeyboardType(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException(nameof(json));

            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<InlineKeyboadData<object>>(json);

            return model.Type;
        }
    }
}