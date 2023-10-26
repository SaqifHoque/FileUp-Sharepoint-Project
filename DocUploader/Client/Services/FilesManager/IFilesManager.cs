using DocUploader.Shared.Dtos;

namespace DocUploader.Client.Services.FilesManager
{
    public interface IFilesManager
    {
        Task<bool> UploadFileChunk(FileChunkDto fileChunkDto);
        Task<bool> UploadFileChunkToSharePoint(FileChunkDto fileChunkDto);
        Task<List<string>> GetFileNames();
    }
}
