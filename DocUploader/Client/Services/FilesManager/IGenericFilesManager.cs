using DocUploader.Shared.Dtos;

namespace DocUploader.Client.Services.FilesManager
{
    public interface IGenericFilesManager
    {
        Task<bool> UploadFileChunk(FileChunkDto fileChunkDto);
        Task<List<string>> GetFileNames();
    }
}
