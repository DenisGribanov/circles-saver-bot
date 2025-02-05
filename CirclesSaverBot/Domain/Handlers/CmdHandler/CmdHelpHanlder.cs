using Domain.Abstractions;
using Domain.Constants;
using Domain.Enums;
using Domain.Models.Telegram;
using Domain.Options;

namespace Domain.Handlers.CmdHandler
{
    public class CmdHelpHanlder : BaseHandler
    {
        public CmdHelpHanlder(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient)
            : base(usersStateService, dataStore, telegramClient)
        {
        }

        protected override Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            return Task.FromResult(BotCommands.HELP.Equals(_messageModel.MessageText));
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            string text = "По вопросам 👉🏻 @gribanov93";

            var inlineQuery = new KeyValuePair<string, string>("Мои кружочки 🔵", string.Empty);

            await _telegramClient.SendVideo(EnvironmentOptionsHelper.EnvironmentOptions.HelpGifFileId, _messageModel.UserFromId, text, inlineQuery);

            return GetHandlerStateName();
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.CMD_HELP;
        }
    }
}