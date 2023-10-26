using DocUploader.Client.Services.SharePoint;
using DocUploader.Server.Data;
using DocUploader.Shared.Dtos;
using DocUploader.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using System.Collections.Concurrent;
using System.IO;
using static Duende.IdentityServer.Models.IdentityResources;

namespace DocUploader.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
  
        private readonly ApplicationDbContext _context;
        private readonly ISharePointService _sharePointService;
        //private readonly ConcurrentDictionary<string, MemoryStream> fileChunks = new ConcurrentDictionary<string, MemoryStream>();
        public FilesController(ApplicationDbContext context, ISharePointService sharePointService)
        {
            _context = context;
            _sharePointService = sharePointService;
        }

        [HttpPost("UploadFileChunk")]
        public async Task<bool> UploadFileChunk([FromBody] FileChunkDto fileChunkDto)
        {
            try
            {
                // get the local filename
                string filePath = Environment.CurrentDirectory + "\\StaticFiles\\";
                string fileName = filePath + fileChunkDto.FileName;

                // delete the file if necessary
                if (fileChunkDto.FirstChunk && System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);

                var splitDocName = fileChunkDto.FileName.Split("_");
                var docName = splitDocName.Last();

                //var uploadedDocuments = await _context.TableModels
                //    .Where(x => x.ClientId == fileChunkDto.ClientId && x.DocumentName.Contains(docName)).ToListAsync();

                //if(uploadedDocuments.Count > 0)
                //{
                //    return false;
                //}



                var extention = Path.GetExtension(fileChunkDto.FileName).ToLower();
                var justFileName = Path.GetFileNameWithoutExtension(fileChunkDto.FileName);
                var (fileName2, fileType) = GetFileNameFileType(fileChunkDto.To!, justFileName);


                var to = fileChunkDto.To!.ToLower();

                var clientsRequest = new ClientsRequest()
                {
                    ClientId = fileChunkDto.ClientId,
                    RequestId = fileChunkDto.RequestId,
                    CreateDate = DateTime.UtcNow

                };

                await _context.ClientsRequests.AddAsync(clientsRequest);
                await _context.SaveChangesAsync();

                string filenameAfterConvertion = await UploadDocumentToS3(fileChunkDto, fileName, fileType);

                if (filenameAfterConvertion != null)
                {
                    return true;
                }

                return false;


            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;

            }


        } //UploadFileChunk

        //[HttpPost("UploadFileChunk")]
        //public async Task<bool> UploadFileChunk([FromBody] FileChunkDto fileChunkDto)
        //{
        //    try
        //    {
        //        // get the local filename
        //        string filePath = Environment.CurrentDirectory + "\\StaticFiles\\";
        //        string fileName = filePath + fileChunkDto.FileName;

        //        // delete the file if necessary
        //        //if (fileChunkDto.FirstChunk && System.IO.File.Exists(fileName))
        //        //    System.IO.File.Delete(fileName);

        //        if (!fileChunks.TryGetValue(fileName, out MemoryStream? memoryStream))
        //        {
        //            memoryStream = new MemoryStream();
        //            fileChunks.TryAdd(fileName, memoryStream);
        //        }

        //        memoryStream.Seek(fileChunkDto.Offset, SeekOrigin.Begin);
        //        await memoryStream.WriteAsync(fileChunkDto.Data!, 0, fileChunkDto.Data!.Length);

        //        if (fileChunkDto.LastChunk)
        //        {
        //            using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
        //            {
        //                memoryStream.Seek(0, SeekOrigin.Begin);
        //                await memoryStream.CopyToAsync(fileStream);
        //            }

        //            memoryStream.Dispose();

        //            var extention = Path.GetExtension(fileChunkDto.FileName).ToLower();
        //            var justFileName = Path.GetFileNameWithoutExtension(fileChunkDto.FileName);
        //            var (fileName2, fileType) = GetFileNameFileType(fileChunkDto.To!, justFileName);


        //            var to = fileChunkDto.To!.ToLower();

        //            var clientsRequest = new ClientsRequest()
        //            {
        //                ClientId = fileChunkDto.ClientId,
        //                RequestId = fileChunkDto.RequestId,
        //                CreateDate = DateTime.UtcNow

        //            };

        //            await _context.ClientsRequests.AddAsync(clientsRequest);
        //            await _context.SaveChangesAsync();

        //            string filenameAfterConvertion = await UploadDocumentToS3(fileChunkDto, fileName, fileType);

        //            if (filenameAfterConvertion != null)
        //            {
        //                return true;
        //            }

        //            return false;
        //        }

        //        return false;

        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        return false;

        //    }


        //} //UploadFileChunk

        [HttpPost("UploadFileChunkToSharePoint")]
        public async Task<bool> UploadFileChunkToSharePoint([FromBody] FileChunkDto fileChunkDto)
        {
            try
            {
                string filePath = Environment.CurrentDirectory + "\\StaticFiles\\";
                string fileName = filePath + fileChunkDto.FileName;

                var fileId = await _sharePointService.UploadFileFromLocalDrive(fileName);

                if (fileId != null)
                {
                    var tableModel = await _context.TableModels.Where(x => x.DocumentName == fileName).FirstOrDefaultAsync();
                    if (tableModel != null)
                    {
                        tableModel.Status = "Uploaded";
                        _context.Update(tableModel);
                        await _context.SaveChangesAsync();
                    }
  
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;

            }
        }



        private async Task<string> UploadDocumentToS3(FileChunkDto fileChunkDto, string fileName, string contentType)
        {
            await Task.Delay(0);
            try
            {
                if (fileChunkDto is null || fileChunkDto.Data!.Length <= 0)
                    return null!;

                //_documentStore = new DocumentStore(_appConfiguration);

                //var result = _documentStore.UploadDocument(file, fileName, contentType);

                // open for writing
                using (var stream = System.IO.File.OpenWrite(fileName))
                {
                    stream.Seek(fileChunkDto.Offset, SeekOrigin.Begin);
                    stream.Write(fileChunkDto.Data!, 0, fileChunkDto.Data!.Length);
                }

                var clientRequest = new ClientsRequest()
                {
                    ClientId =  fileChunkDto.ClientId,
                    RequestId = fileChunkDto.RequestId,
                    CreateDate = DateTime.UtcNow
                };

                await _context.ClientsRequests.AddAsync(clientRequest);
                await _context.SaveChangesAsync();

                var uploadedDoc = new UploadedDocuments()
                {
                    // Client Id
                    ClientRequestId = clientRequest.ClientRequestId,
                    DocumentPath = fileName
                };

                await _context.UploadedDocuments.AddAsync(uploadedDoc);
                await _context.SaveChangesAsync();

                var requestDoc = new RequestDocuments()
                {
                    RequestId = fileChunkDto.RequestId,
                    DocumentName = fileName,
                };

                await _context.RequestDocuments.AddAsync(requestDoc);
                await _context.SaveChangesAsync();

                var tableModel = new TableModel()
                {
                    ClientId = fileChunkDto.ClientId,
                    RequestId = fileChunkDto.RequestId,
                    DocumentName = fileName,
                    Status = "Pending",
                    DateUploaded = DateTime.UtcNow
                };

                await _context.TableModels.AddAsync(tableModel);
                await _context.SaveChangesAsync();



                return fileName;
            }
            catch (Exception)
            {
                return null!;
            }
        }


        private static (string _fileName, string _fileType) GetFileNameFileType(string to, string justFileName)
        {
            ValueTuple<string, string> FileNameFileType = ("", "");
            var fileName = "";
            var fileType = "";

            if (to == "jpg")
            {
                fileName = justFileName + ".jpg";
                fileType = "image/jpeg";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;

            }

            else if (to == "png")
            {
                fileName = justFileName + ".png";
                fileType = "image/png";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;
            }

            else if (to == "svg")
            {
                fileName = justFileName + ".svg";
                fileType = "image/svg";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;
            }

            else if (to == "jpeg")
            {
                fileName = justFileName + ".jpeg";
                fileType = "image/jpeg";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;
            }
            else if (to == "bmp")
            {
                fileName = justFileName + ".bmp";
                fileType = "image/bmp";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;
            }
            else if (to == "eps")
            {
                fileName = justFileName + ".eps";
                fileType = "application/postscript";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;
            }
            else if (to == "gif")
            {
                fileName = justFileName + ".gif";
                fileType = "image/gif";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;
            }
            else if (to == "tiff")
            {
                fileName = justFileName + ".tiff";
                fileType = "image/tiff";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;
            }
            else if (to == "webp")
            {
                fileName = justFileName + ".webp";
                fileType = "image/webp";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;
            }
            else if (to == "pdf")
            {
                fileName = justFileName + ".pdf";
                fileType = "application/pdf";
                FileNameFileType = (fileName, fileType);
                return FileNameFileType;
            }

            return FileNameFileType;
        }

        [HttpGet("getclients")]
        public async Task<IEnumerable<Shared.Models.Client>> GetClients()
        {
            var clients = await _context.Clients.ToListAsync();

            return clients;
        }

        [HttpGet("getrequests")]
        public async Task<IEnumerable<Request>> GetRequests()
        {
            var requests = await _context.Requests.ToListAsync();

            return requests;
        }

        [HttpGet("getUploadedFileByClientId/{id?}")]
        public async Task<IEnumerable<TableModel>> GetDocuments(int id)
        {
            
            var uploadedDocuments = await _context.TableModels
                .Where(x => x.ClientId == id).OrderByDescending(x => x.Id).ToListAsync();

            return uploadedDocuments;

        }

        [HttpGet("getUploadedFileByRequestId/{id?}")]
        public async Task<IEnumerable<TableModel>> GetDocumentsByRequest(int id)
        {

            

            var requestDocs = await _context.RequestDocuments
                .Where(x => x.RequestId == id).FirstOrDefaultAsync();

            var docs = await _context.TableModels
                .Where(x => x.RequestId == id).FirstOrDefaultAsync();

           var uploadedDocuments = await _context.TableModels
                .Where(x => x.RequestId == id).ToListAsync();

                return uploadedDocuments;
           
            

        }

        [HttpGet("getclientsId/{email?}")]
        public async Task<int> GetClientId(string email)
        {
            var client = await _context.Clients.Where(x => x.Email == email).FirstOrDefaultAsync();
            return client!.ClientId;
        
        }

        [HttpGet("downloadFile/{fileName?}")]
        public async Task<Stream> DownloadFile(string fileName)
        {
            var file = await _sharePointService.DownloadFileByName(fileName);
            return file;

        }


    }
}
