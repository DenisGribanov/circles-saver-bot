using Domain.Abstractions;
using Domain.Models;
using RestSharp;

namespace Infrastructure.Clients
{
    public class VideoStickersBotClient : IVideoStickersBotClient
    {
        public readonly string BaseUrl;
        private readonly RestClient client;

        public VideoStickersBotClient(string baseUrl)
        {
            BaseUrl = baseUrl;
            client = new RestClient(BaseUrl);
        }

        public async Task<VideoStickersInfoResponse> GetStickerDescription(string fileUniqueId)
        {
            RestRequest restRequest = new RestRequest()
            {
                Resource = "/sticker",
                Method = Method.Get
            };

            restRequest.AddParameter("fileUniqueId", fileUniqueId);

            var response = await client.ExecuteAsync(restRequest);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка при вызове API VideoStickersBot status : {response.StatusCode}");
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<VideoStickersInfoResponse>(response.Content);
        }
    }
}