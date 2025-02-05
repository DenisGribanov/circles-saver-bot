namespace Domain.Options
{
    public class EnvironmentOptions
    {
        public const string SectionMame = "Environment";

        public string BotSecret { get; init; }

        public string HelpGifFileId { get; init; }

        public string VideoStickerBotUrl { get; init; }

        public string VideoResizeUrl { get; init; }

        public int MaxVideoDuration { get; init; }

        public int MaxFileSize { get; init; }
    }
}