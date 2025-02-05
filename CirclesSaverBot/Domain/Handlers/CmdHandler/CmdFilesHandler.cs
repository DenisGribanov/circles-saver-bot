using Domain.Abstractions;
using Domain.Constants;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers.CmdHandler
{
    public class CmdFilesHandler : BaseHandler
    {
        public CmdFilesHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient)
            : base(usersStateService, dataStore, telegramClient)
        {
        }

        protected override Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            return Task.FromResult(BotCommands.FILES.Equals(_messageModel.MessageText));
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            var inlineQuery = new KeyValuePair<string, string>("👉🏻 Жми сюда", string.Empty);

            string text = "Выберите кружочек для редактирования 👇";

            await _telegramClient.SendTextMessage(text, _messageModel.UserFromId, inlineQuery);

            return GetHandlerStateName();
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.CMD_FILES;
        }
    }
}