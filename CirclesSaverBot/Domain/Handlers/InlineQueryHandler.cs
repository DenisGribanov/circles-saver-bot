using Domain.Abstractions;
using Domain.Enums;
using Domain.Models.Telegram;

namespace Domain.Handlers
{
    public class InlineQueryHandler : BaseHandler
    {
        private const int MAX_RESULT = 50;
        private readonly ISearchService _searchService;

        public InlineQueryHandler(IUsersStateService usersStateService,
            IDataStore dataStore,
            ITelegramClient telegramClient,
            ISearchService searchService) : base(usersStateService, dataStore, telegramClient)
        {
            _searchService = searchService;
        }

        protected override async Task<UserStateTypeEnum?> HandleInternal(TelegramMessageModel _messageModel)
        {
            int? offset = null;

            if (int.TryParse(_messageModel.InlineQueryOffset, out var val))
            {
                offset = val;
            }

            var searchResult = await _searchService.Search(_messageModel.UserFromId, MAX_RESULT, _messageModel.InlineQueryText, offset);

            if (searchResult == null || searchResult.Pictures == null || searchResult.Pictures.Count == 0)
            {
                await _telegramClient.EmptyAnswerInlineQuery(_messageModel.UserFromId, _messageModel.InlineQueryId);
            }
            else
            {
                await _telegramClient.AnswerInlineQuery(_messageModel.UserFromId, _messageModel.InlineQueryId, searchResult.Pictures, searchResult.NextOffset?.ToString());
            }

            return UpdateStateForCurrentUser(_messageModel.UserFromId);
        }

        protected override Task<bool> MatchInternal(TelegramMessageModel _messageModel)
        {
            return Task.FromResult(_messageModel.IsInlineQuery && !string.IsNullOrEmpty(_messageModel.InlineQueryId));
        }

        protected override UserStateTypeEnum GetHandlerStateName()
        {
            return UserStateTypeEnum.INLINE_QUERY;
        }
    }
}