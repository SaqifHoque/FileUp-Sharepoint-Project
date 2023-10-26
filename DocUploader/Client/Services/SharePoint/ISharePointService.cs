using Microsoft.Graph;

namespace DocUploader.Client.Services.SharePoint
{
    public interface ISharePointService
    {
        Task<Stream> DownloadFile(string fileId);
        void DeleteDriveItem(string siteId, string driveId, string itemId);
        Task<string> UploadFileFromLocalDrive(string filePath, string folderId = null);
        Task<Stream> DownloadFileByName(string fileName);
    }
}
