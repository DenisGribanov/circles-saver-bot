using Domain.Abstractions;
using Domain.Constants;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers.CmdHandler
{
    public class CmdStartHandler : BaseHandler
    {
        public CmdStartHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient)
            : base(usersStateService, dataStore, telegramClient)
        {
        }

        protected override Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            return Task.FromResult(!string.IsNullOrEmpty(_messageModel.MessageText) &&
                        _messageModel.MessageText.StartsWith(BotCommands.START));
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            string text = "Я умею хранить кружочки 🔵, для их быстрого использования 🔎 через inline режим, как бот @VideoStickersBot\r\n\r\n" +
                        "Пришли 📤 мне кружочек (или видео 🎞, а я сделаю кружочек сам) и добавлю его в твой список (только ты видишь свои кружочки 👀).\r\n\r\n" +
                        "🆘 Хочешь узнать как мной пользоваться ? жми сюда 👉🏻 /help";

            var inlineQuery = new KeyValuePair<string, string>("Мои кружочки 🔵", string.Empty);

            await _telegramClient.SendTextMessage(text, _messageModel.UserFromId, inlineQuery);

            return GetHandlerStateName();
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.CMD_START;
        }
    }
}