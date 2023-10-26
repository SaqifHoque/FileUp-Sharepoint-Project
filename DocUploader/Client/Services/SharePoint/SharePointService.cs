using Azure.Identity;
using DocUploader.Shared.Dtos;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using static System.Net.WebRequestMethods;

namespace DocUploader.Client.Services.SharePoint
{
    public class SharePointService : ISharePointService
    {
        private GraphServiceClient _graphClient;
        private readonly SharePointConfiguration _sharePointConfig;
        private string _driveId;
        private string _siteId;

        /// <summary>
        /// Write status message Event.
        /// </summary>
        public Action<string> WriteLogEvent;

        private void WriteLog(string message)
        {
            if (WriteLogEvent != null)
                WriteLogEvent(message);
        }

        /// <summary>
        /// Constructs a new <see cref="SharePointService"></see> instance
        /// </summary>
        public SharePointService(SharePointConfiguration sharePointConfig)
        {
            _sharePointConfig = sharePointConfig;
            _driveId = _sharePointConfig.DriveId;
            _siteId = _sharePointConfig.SiteId;
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                 _sharePointConfig.TenantId,
                 _sharePointConfig.ApplicationId,
                 _sharePointConfig.ClientSecret,
                 options);

            _graphClient = new GraphServiceClient(clientSecretCredential, scopes);
            //var allsites = BrowseSites(recursive: true);
            //var site = GetSite(allsites[0].Id);
            //var drives = BrowseSiteDrives(site.Id);

        }

        public async Task<Stream> DownloadFile(string fileId)
        {
            return _graphClient
                .Sites[_siteId]
                .Drives[_driveId]
                .Items[fileId]
                .Content
                .Request().GetAsync().GetAwaiter().GetResult();
        }

        public async Task<Stream> DownloadFileByName(string fileName)
        {
            var drive = _graphClient.Sites[_siteId].Drives[_driveId];

            // Search for the file by its name
            var queryOptions = new List<QueryOption>
            {
                new QueryOption("filter", $"name eq '{fileName}'")
            };

            var items = await drive.Root.Children.Request(queryOptions).GetAsync();
            var file = items.FirstOrDefault();

            if (file != null)
            {
                var contentStream = await drive.Items[file.Id].Content.Request().GetAsync();
                var memoryStream = new MemoryStream();
                await contentStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset the memory stream position to the beginning
                return memoryStream;

            }
            else
            {
                // Handle the case when the file with the specified filename is not found
                throw new FileNotFoundException("File not found in SharePoint.");
            }
        }


        /// <summary>
        /// Deletes Drive item: file or folder.
        /// </summary>
        /// <param name="siteId">Site ID</param>
        /// <param name="driveId">Drive ID</param>
        /// <param name="itemId">Item ID</param>
        public void DeleteDriveItem(string siteId, string driveId, string itemId)
        {
            WriteLog($"Deleting file ID='{itemId}'");

            _graphClient
               .Sites[siteId]
               .Drives[driveId]
               .Items[itemId]
               .Request()
               .DeleteAsync()
               .GetAwaiter()
               .GetResult();
        }

        public List<Site> BrowseSites(string parentSiteId = "", bool recursive = false)
        {
            WriteLog($"Browse {(parentSiteId ?? "Root")} sites {(recursive ? "recursively" : "")}");
            var sites = new List<Site>();

            if (string.IsNullOrEmpty(parentSiteId))
            {
                sites = _graphClient.Sites.Request().GetAsync().GetAwaiter().GetResult().ToList();
            }
            else
            {
                sites = _graphClient.Sites[parentSiteId].Sites.Request().GetAsync().GetAwaiter().GetResult().ToList();
            }

            if (sites == null)
                return new List<Site>();

            if (recursive)
            {
                var chiledSites = new List<Site>();
                foreach (Site s in sites)
                {
                    s.Drives = BrowseSiteDrives(s.Id);
                    chiledSites.AddRange(BrowseSites(s.Id, true));
                }
                sites.AddRange(chiledSites);
            }
            else
            {
                foreach (Site s in sites)
                {
                    s.Drives = BrowseSiteDrives(s.Id);
                }
            }
            return sites;
        }

        /// <summary>
        /// Get a Site by ID. (Not recursive)
        /// </summary>
        /// <param name="siteId">Site ID</param>
        /// <returns><see cref="Site"/> object instance</returns>
        public Site GetSite(string siteId)
        {
            if (string.IsNullOrEmpty(siteId))
            {
                throw new ArgumentNullException(siteId);
            }

            var site = _graphClient.Sites[siteId].Request().GetAsync().GetAwaiter().GetResult();
            site.Drives = BrowseSiteDrives(site.Id);

            return site;
        }

        /// <summary>
        /// Get Site Drives
        /// </summary>
        /// <param name="siteId">Site ID</param>
        /// <returns><see cref="ISiteDrivesCollectionPage"/> object</returns>
        public ISiteDrivesCollectionPage BrowseSiteDrives(string siteId)
        {
            if (string.IsNullOrEmpty(siteId))
            {
                throw new ArgumentNullException(siteId);
            }
            WriteLog($"Browse {siteId} site drives");

            return _graphClient
                   .Sites[siteId].Drives
                   .Request()
                   .GetAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets files and folders from Document Library
        /// </summary>
        /// <param name="siteId">Site ID</param>
        /// <param name="driveId">Drive ID</param>
        /// <param name="onlyFolders">Show Only Folders</param>
        /// <param name="folderId">Folder ID</param>
        /// <returns>List of <see cref="DriveItem"/></returns>
        public List<DriveItem> BrowseDriveItems(string siteId, string driveId, bool onlyFolders = false, string folderId = "root")
        {
            if (string.IsNullOrEmpty(siteId))
            {
                throw new ArgumentNullException(siteId);
            }
            if (string.IsNullOrEmpty(driveId))
            {
                throw new ArgumentNullException(driveId);
            }

            var options = new List<Option>
            {
               new QueryOption("select", "Name,Id,WebUrl,size")
            };

            if (onlyFolders)
                options.Add(new QueryOption("filter", "Folder ne null"));

            var allitems = new List<DriveItem>();
            var items = _graphClient
                .Sites[siteId]
                .Drives[driveId]
                .Items[folderId]
                .Children
                .Request(options)
                .GetAsync()
                .GetAwaiter()
                .GetResult();

            allitems.AddRange(items);
            foreach (var i in items)
            {
                var itms = BrowseDriveItems(siteId, driveId, onlyFolders: onlyFolders, folderId: i.Id);
                allitems.AddRange(itms);
            }

            return allitems;
        }

        public async Task<string> UploadFileFromLocalDrive(string filePath, string folderId = null)
        {
            var fi = new FileInfo(filePath);
            if (!fi.Exists)
                throw new FileNotFoundException("File not found", filePath);
            if (fi.Length >= 268435456000) //250GB
                throw new Exception("Sharepoint does not allow uploading more than 250GB https://docs.microsoft.com/en-us/office365/servicedescriptions/sharepoint-online-service-description/sharepoint-online-limits");

            var requestModel = new UploadFileRequestModel
            {
                SiteId = _siteId,
                DriveId = _driveId,
                FolderId = folderId ?? "root",
                FileLocalPath = filePath,
                FileInfo = new FileInfo(filePath)
            };

            string newFileId = string.Empty;
            if (requestModel.FileInfo.Length < 15728640) // 15Mb
                newFileId = await GetSmallFileUploadResponse(requestModel);
            else
                newFileId = await GetLargeFileUploadResponse(requestModel);

            return newFileId.Replace("\"", string.Empty).Split(new char[] { ',' }).First();
        }

        private async Task<string> GetSmallFileUploadResponse(UploadFileRequestModel requestModel)
        {
            WriteLog($"Small File Upload: {requestModel.FileLocalPath}");
            string etag = string.Empty;

            using (Stream stream = requestModel.FileInfo.OpenRead())
            {
                etag = _graphClient
                        .Drives[requestModel.DriveId]
                        .Items[requestModel.FolderId]
                        .ItemWithPath(requestModel.FileInfo.Name)
                        .Content
                        .Request()
                        .PutAsync<DriveItem>(stream)
                        .GetAwaiter()
                        .GetResult().ETag;

            }

            return etag;
        }

        private async Task<string> GetLargeFileUploadResponse(UploadFileRequestModel requestModel)
        {
            WriteLog($"Large File Upload: {requestModel.FileLocalPath}");
            using (Stream stream = requestModel.FileInfo.OpenRead())
            {
                var uploadProps = new DriveItemUploadableProperties
                {
                    AdditionalData = new Dictionary<string, object>
                {
                    { "@microsoft.graph.conflictBehavior", "replace" }
                }
                };

                var uploadSession = _graphClient
                    .Drives[requestModel.DriveId]
                    .Items[requestModel.FolderId]
                    .ItemWithPath(requestModel.FileInfo.Name)
                    .CreateUploadSession(uploadProps)
                    .Request()
                    .PostAsync()
                    .GetAwaiter()
                    .GetResult();

                // Max slice size must be a multiple of 320 KB
                int maxSliceSize = 320 * 1024;
                var fileUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, stream, maxSliceSize);

                try
                {
                    IProgress<long> uploadProgress = new Progress<long>(uploadBytes =>
                    {
                        WriteLog($"Uploaded {uploadBytes} bytes of {stream.Length} bytes");
                    });

                    // Upload the file
                    var uploadResult = fileUploadTask.UploadAsync(uploadProgress).GetAwaiter().GetResult();
                    return uploadResult.ItemResponse.ETag;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error uploading {requestModel.FileLocalPath}", ex);
                }
            }
        }
    }

}
