using DocUploader.Server.Data;
using DocUploader.Shared.Dtos;
using DocUploader.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocUploader.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
  
        private readonly ApplicationDbContext _context;
        public FilesController(ApplicationDbContext context)
        {
            _context = context;

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


                var extention = Path.GetExtension(fileChunkDto.FileName).ToLower();
                var justFileName = Path.GetFileNameWithoutExtension(fileChunkDto.FileName);
                var (fileName2, fileType) = GetFileNameFileType(fileChunkDto.To!, justFileName);


                var to = fileChunkDto.To!.ToLower();

                //var converted = ConverterEngine(fileChunkDto.Data!, to);
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
                    ClientRequestId = clientRequest.ClientRequestId,
                    DocumentPath = fileName
                };

                await _context.UploadedDocuments.AddAsync(uploadedDoc);
                await _context.SaveChangesAsync();

                var requestDoc = new RequestDocuments()
                {
                    RequestId = fileChunkDto.RequestId,
                    DocumentName = fileName
                };

                await _context.RequestDocuments.AddAsync(requestDoc);
                await _context.SaveChangesAsync();

                var tableModel = new TableModel()
                {
                    ClientId = fileChunkDto.ClientId,
                    RequestId = fileChunkDto.RequestId,
                    DocumentName = fileName,
                    Status = "Uploaded",
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
                .Where(x => x.ClientId == id).ToListAsync();

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


    }
}
