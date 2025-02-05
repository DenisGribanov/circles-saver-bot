using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Models.Telegram;
using System.Text;

namespace Domain.Handlers.FileHandler
{
    public class ExistVideoNoteHandler : BaseHandler
    {
        private TgMediaFile? TgMediaFile;

        public ExistVideoNoteHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient) : base(usersStateService, dataStore, telegramClient)
        {
        }

        protected override async Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            if (_messageModel.TelegramFile == null)
            {
                return false;
            }

            TgMediaFile = await _dataStore.GetTgMediaFile(_messageModel.UserFromId, _messageModel.TelegramFile.FileUniqueId);

            return TgMediaFile != null;
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            var inlineQuery = new KeyValuePair<string, string>("Посмотреть 👀", TgMediaFile.Number.ToString());
            var inlineKeyboard = GeInlineKeyBoardData(TgMediaFile.Id);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"id: {TgMediaFile.Number}\r\n\r\n");
            stringBuilder.Append($"🔎 _Описание (для поиска):_ {TgMediaFile.Description}\r\n\r\n");

            await _telegramClient.SendTextMessage(stringBuilder.ToString(), _messageModel.UserFromId, inlineQuery, inlineKeyboard);

            return GetHandlerStateName();
        }

        private static Dictionary<string, string> GeInlineKeyBoardData(long tgFileId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            Dictionary<ManagmentActionEnum, string> keys = new Dictionary<ManagmentActionEnum, string>
            {
                { ManagmentActionEnum.EDIT_DESCRIPTION, "Изменить описание 🔎" },
                { ManagmentActionEnum.REMOVE, "Удалить 🗑" }
            };

            foreach (var pair in keys)
            {
                var managmentMode = InlineManagmentMode.Init(pair.Key, tgFileId);
                var keyboadBase = InlineKeyboadData<InlineManagmentMode>.Init(InlineKeyboardTypeEnum.MANAGMENT_MODE, managmentMode);
                var json = keyboadBase.ToJson();

                result.Add(pair.Value, json);
            }

            return result;
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.VIDEO_NOTE_EXIST;
        }
    }
}