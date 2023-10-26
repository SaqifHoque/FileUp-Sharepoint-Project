using DocUploader.Client.Services.FilesManager;
using DocUploader.Shared.Dtos;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace DocUploader.Client.Pages
{
    public partial class FileUploader
    {
        [Inject] IFilesManager? FilesManager { get; set; }
        private bool isUploading = false;
        private bool Converted = false;
        private string ErrorMessage = string.Empty;
        private string isDisplaying = "block";

        private string dropClass = string.Empty;
        private int maxAllowedFiles = 1000;
        //List<string> FileUrls = new List<string>();
        List<FileUploadProgress> filesQueue = new();
        List<UpdatedFileInfo> UpdatedFileInfos = new List<UpdatedFileInfo>();

        private void AddFilesToQueue(InputFileChangeEventArgs e, string docType)
        {
            docTable = false;
            dropClass = string.Empty;
            ErrorMessage = string.Empty;

            if (e.FileCount > maxAllowedFiles)
            {
                ErrorMessage = $"A maximum of {maxAllowedFiles} is allowed, you have selected {e.FileCount} files!";
            }
            else
            {
                var files = e.GetMultipleFiles(maxAllowedFiles);
                var fileCount = filesQueue.Count;

                foreach (var file in files)
                {
                    bool containsSubstring = filesQueue.Any(obj => obj.DocType == docType);
                    if (!containsSubstring)
                    {
                        var progress = new FileUploadProgress(file, file.Name, file.Size, fileCount, docType);
                        filesQueue.Add(progress);
                        fileCount++;
                    }
                    
                }
            }
        } //PlaceFilesInQue


        private async Task UploadFileQueue(string convertTo, int clientId, int requestId)
        {
            isUploading = true;
            await InvokeAsync(StateHasChanged);


            foreach (var file in filesQueue.OrderByDescending(x => x.FileId))
            {
                if (!file.HasBeenUploaded)
                {
                    file.To = convertTo;
                    file.ClientId = clientId;
                    file.RequestId = requestId;
                    file.IsUploading = true;
                    var uc = Task.Run(async () => await UploadChunks(file));
                                        
                }
            }

            //isUploading = false;
            //await InvokeAsync(StateHasChanged);
        } //UploadFileQueue

        private async Task UploadChunksToSharepoints(FileUploadProgress file, string fileName)
        {
            await Task.Run(async () =>
            {
                var TotalBytes = file.Size;
                //long chunkSize = 400000;
                long chunkSize = long.MaxValue;
                long numChunks = TotalBytes / chunkSize;
                long remainder = TotalBytes % chunkSize;
                string to = file.To!;

                bool firstChunk = true;
                using (var inStream = file.FileData.OpenReadStream(long.MaxValue))
                {
                    for (int i = 0; i < numChunks; i++)
                    {
                        var buffer = new byte[chunkSize];
                        await inStream.ReadAsync(buffer, 0, buffer.Length);

                        var chunk = new FileChunkDto
                        {
                            Data = buffer,
                            FileName = fileName,
                            Offset = filesQueue[file.FileId].UploadedBytes,
                            FirstChunk = firstChunk,
                            To = to,
                            ClientId = file.ClientId,
                            RequestId = file.RequestId
                        };

                        bool success = await FilesManager!.UploadFileChunkToSharePoint(chunk);

                        if (success)
                        {
                            Converted = true;
                            isDisplaying = "none";
                            await InvokeAsync(StateHasChanged);

                        }

                        firstChunk = false;

                        // Update our progress data and UI
                        filesQueue[file.FileId].UploadedBytes += chunkSize;
                        await InvokeAsync(StateHasChanged);
                    }

                    if (remainder > 0)
                    {
                        var buffer = new byte[remainder];
                        await inStream.ReadAsync(buffer, 0, buffer.Length);

                        var chunk = new FileChunkDto
                        {
                            Data = buffer,
                            FileName = fileName,
                            Offset = filesQueue[file.FileId].UploadedBytes,
                            FirstChunk = firstChunk,
                            To = to,
                            ClientId = file.ClientId,
                            RequestId = file.RequestId
                        };
                        bool success = await FilesManager!.UploadFileChunkToSharePoint(chunk);

                        if (success)
                        {
                            Converted = true;
                            isDisplaying = "none";
                            await InvokeAsync(StateHasChanged);

                        }

                        // Update our progress data and UI
                        filesQueue[file.FileId].UploadedBytes += remainder;
                        file.IsPending = false;
                        file.HasBeenUploaded = true;
                        //await ListFiles();
                        await InvokeAsync(StateHasChanged);
                        //return newFileNameWithoutPath;
                    }
                }
            });
        } //UploadChunks


        private async Task UploadChunks(FileUploadProgress file)
        {
            await Task.Run(async () =>
            {
                isUploading = true;
                await InvokeAsync(StateHasChanged);
                var TotalBytes = file.Size;
                //long chunkSize = 4000000;
                long chunkSize = long.MaxValue;
                long numChunks = TotalBytes / chunkSize;
                long remainder = TotalBytes % chunkSize;
                string to = file.To!;

                string nameOnly = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                string newFileNameWithoutPath = $"{DateTime.Now.Ticks}_{file.ClientId}_{file.RequestId}_{file.DocType}{extension}";

                bool firstChunk = true;
                using (var inStream = file.FileData.OpenReadStream(long.MaxValue))
                {
                
                    for (int i = 0; i < numChunks; i++)
                    {
                        
                        var buffer = new byte[chunkSize];
                        await inStream.ReadAsync(buffer, 0, buffer.Length);

                        var chunk = new FileChunkDto
                        {
                            Data = buffer,
                            FileName = newFileNameWithoutPath,
                            //Offset = filesQueue[file.FileId].UploadedBytes,
                            FirstChunk = firstChunk,
                            LastChunk = false,
                            To = to,
                            ClientId = file.ClientId,
                            RequestId = file.RequestId
                        };
                        _ = Task.Run(async () =>
                        {
                            bool success = await FilesManager!.UploadFileChunk(chunk);
                            if (success)
                            {
                                isUploading = false;
                                file.IsPending = true;
                                file.IsUploading = false;
                                isUploading = false;
                                StateHasChanged();

                                Converted = true;
                                isDisplaying = "none";
                                await InvokeAsync(StateHasChanged);

                                UpdatedFileInfo obj = new UpdatedFileInfo
                                {
                                    UpdatedFileName = newFileNameWithoutPath
                                };

                                UpdatedFileInfos.Add(obj);
                            }
                            else
                            {
                                isUploading = false;
                                await InvokeAsync(StateHasChanged);
                            }
                        });

                        firstChunk = false;

                        // Update our progress data and UI
                        //filesQueue[file.FileId].UploadedBytes += chunkSize;
                        await InvokeAsync(StateHasChanged);
                        
                    }

                    if (remainder > 0)
                    {
                        var buffer = new byte[remainder];
                        await inStream.ReadAsync(buffer, 0, buffer.Length);

                        var chunk = new FileChunkDto
                        {
                            Data = buffer,
                            FileName = newFileNameWithoutPath,
                            //Offset = filesQueue[file.FileId].UploadedBytes,
                            FirstChunk = firstChunk,
                            LastChunk = true,
                            To = to,
                            ClientId = file.ClientId,
                            RequestId = file.RequestId
                        };
                        _ = Task.Run(async () =>
                        {
                            bool success = await FilesManager!.UploadFileChunk(chunk);

                            if (success)
                            {
                                isUploading = false;
                                file.IsPending = true;
                                file.IsUploading = false;
                                StateHasChanged();
                                var ucs = Task.Run(async () => await UploadChunksToSharepoints(file, newFileNameWithoutPath));
                                await ucs.ContinueWith(_ =>
                                {
                                    file.IsPending = false;
                                    file.HasBeenUploaded = true;
                                });

                                Converted = true;
                                isDisplaying = "none";
                                await InvokeAsync(StateHasChanged);


                                UpdatedFileInfo obj = new UpdatedFileInfo
                                {
                                    UpdatedFileName = newFileNameWithoutPath
                                };

                                UpdatedFileInfos.Add(obj);


                            } else {
                                isUploading = false;
                                file.IsFailed = true;
                                file.IsPending = false;
                                file.HasBeenUploaded = false;
                                await InvokeAsync(StateHasChanged);
                            }
                        });

                        // Update our progress data and UI
                        //filesQueue[file.FileId].UploadedBytes += remainder;
                        //await ListFiles();

                        await InvokeAsync(StateHasChanged);
                    }
                
                }
            });
        } //UploadChunks

        private async Task GoBack()
        {
            Converted = false;
            isDisplaying = "block";
            UpdatedFileInfos.Clear();
            filesQueue.Clear();
            await InvokeAsync(StateHasChanged);
        }


        private void RemoveFromQueue(int fileId)
        {
            var itemToRemove = filesQueue.SingleOrDefault(x => x.FileId == fileId);
            if (itemToRemove != null)
                filesQueue.Remove(itemToRemove);
        } //RemoveFromQueue


        private void ClearFileQueue()
        {
            filesQueue.Clear();
        } //ClearFileQueue        


        record FileUploadProgress(IBrowserFile File, string FileName, long Size, int FileId, string DocType)
        {
            public IBrowserFile FileData { get; set; } = File;
            public int FileId { get; set; } = FileId;
            public string? DocType { get; set; } = DocType;
            public long UploadedBytes { get; set; }
            public double UploadedPercentage => (double)UploadedBytes / (double)Size * 100d;
            public bool HasBeenUploaded { get; set; } = false;
            public bool IsPending { get; set; } = false;
            public bool IsUploading { get; set; } = false;
            public bool IsFailed { get; set; } = false;
            public string? To { get; set; }
            public int ClientId { get; set; }
            public int RequestId { get; set; }
        } //FileUploadProgress




        void HandleDragEnter()
        {
            dropClass = "dropzone-active";
        } //HandleDragEnter
        void HandleDragLeave()
        {
            dropClass = string.Empty;
        } //HandleDragLeave


        /*
        protected override async Task OnInitializedAsync()
        {
            await ListFiles();
        }

        private async Task ListFiles()
        {
            FileUrls = await FilesManager.GetFileNames();
            await InvokeAsync(StateHasChanged);
        }
        */



    }
}
