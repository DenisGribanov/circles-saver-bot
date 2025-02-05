using Domain.Abstractions;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers.InlineCallBack
{
    public abstract class ManagmentCallBackBaseHandler : BaseHandler
    {
        private InlineKeyboadData<InlineManagmentMode> KeyboadCallBackData;
        protected abstract ManagmentActionEnum ManagmentAction { get; }

        protected const InlineKeyboardTypeEnum KeyboardType = InlineKeyboardTypeEnum.MANAGMENT_MODE;

        protected readonly IManagmentStateService _managmentStateService;

        public ManagmentCallBackBaseHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient,
            IManagmentStateService managmentStateService) : base(usersStateService, dataStore, telegramClient)
        {
            _managmentStateService = managmentStateService;
        }

        protected override Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            if (!_messageModel.IsCallBackQuery || string.IsNullOrEmpty(_messageModel.CallBackData))
            {
                return Task.FromResult(false);
            }

            if (GetKeyboardType(_messageModel.CallBackData) == KeyboardType)
            {
                KeyboadCallBackData = InlineKeyboadData<InlineManagmentMode>.FromJson(_messageModel.CallBackData);
                return Task.FromResult(KeyboadCallBackData.Data.Action == ManagmentAction);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}