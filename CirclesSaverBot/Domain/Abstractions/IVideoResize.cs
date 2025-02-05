namespace Domain.Abstractions
{
    public interface IVideoResize
    {
        Task<byte[]> ConvertToSquareAsync(MemoryStream sourceVideo);
    }
}