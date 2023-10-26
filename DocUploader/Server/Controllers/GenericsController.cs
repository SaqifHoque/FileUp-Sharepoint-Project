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
    public class GenericsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GenericsController(ApplicationDbContext context)
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
                


                var to = fileChunkDto.To!.ToLower();





                string uploaded = await UploadDocumentToS3(fileChunkDto, fileName);

                if (uploaded != null)
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



        private async Task<string> UploadDocumentToS3(FileChunkDto fileChunkDto, string fileName)
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


                var genericModel = new GenericModel()
                {
                    ClientId = fileChunkDto.ClientId,
                    DocumentName = fileName,
                    Status = "Uploaded",
                    DateUploaded = DateTime.UtcNow
                };

                await _context.GenericModels.AddAsync(genericModel);
                await _context.SaveChangesAsync();



                return fileName;
            }
            catch (Exception)
            {
                return null!;
            }
        }


       

        [HttpGet("getclients")]
        public async Task<IEnumerable<Shared.Models.Client>> GetClients()
        {
            var clients = await _context.Clients.ToListAsync();

            return clients;
        }


        [HttpGet("getUploadedFileByClientId/{id?}")]
        public async Task<IEnumerable<GenericModel>> GetDocuments(int id)
        {

            var uploadedDocuments = await _context.GenericModels
                .Where(x => x.ClientId == id).ToListAsync();

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
