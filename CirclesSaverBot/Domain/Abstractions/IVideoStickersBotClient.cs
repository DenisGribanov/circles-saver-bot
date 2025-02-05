using Domain.Models;

namespace Domain.Abstractions
{
    public interface IVideoStickersBotClient
    {
        Task<VideoStickersInfoResponse> GetStickerDescription(string fileUniqueId);
    }
}