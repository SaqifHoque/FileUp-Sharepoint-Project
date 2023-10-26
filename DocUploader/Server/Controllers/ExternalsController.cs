using DocUploader.Server.Data;
using DocUploader.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Duende.IdentityServer.Models.IdentityResources;

namespace DocUploader.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExternalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("client")]
        public async Task<IActionResult> PostClient(ExternalClient client)
        {
            if (ModelState.IsValid)
            {
                var obj = new DocUploader.Shared.Models.Client()
                {
                    ClientName = client.ClientName,
                    Email  = client.Email,
                    Last4Digits = client.Last4Digits,
                    Password = client.Password,
                    ClientIdInternal = client.ClientIdInternal
                };

                await _context.Clients!.AddAsync(obj);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest(client);
            }
        }


        [HttpPost("post-document")]
        public async Task<IActionResult> 
            PostDocument(ExternalRequestDocument requestDocument)
        {
            if (ModelState.IsValid)
            {
                var obj = new RequestDocuments()
                {
                    RequestId = requestDocument.ExternalRequestId,
                    DocumentName = requestDocument.DocumentsName,
                };
                await _context.RequestDocuments!.AddAsync(obj);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest(requestDocument);
            }
        }

        [HttpPost("request")]
        public async Task<IActionResult> PostRequest(ExternalRequest request)
        {
            if (ModelState.IsValid)
            {
                var obj = new Request
                {
                    RequestIdInternal = request.RequestIdInternal,
                    RequestName = request.RequestName
                };
                await _context.Requests!.AddAsync(obj);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest(request);
            }
        }

        [HttpGet("documents/{clientIDInternal?}/{requestIDInternal?}")]
        public async Task<ActionResult<IEnumerable<ExternalRequestDocument>>>
            GetDocuments(int clientIDInternal, int requestIDInternal)
        {
            var records = await _context.RequestDocuments!
                .Where(x => x.RequestId == requestIDInternal)
                .Select(x => new ExternalRequestDocument
                {
                    DocumentIdInternal = x.RequestDocumentId,
                    DocumentsPath = x.DocumentName,
                    DocumentsName = x.DocumentName
                    
                }).ToListAsync();

            return Ok(records);
        }

        [HttpGet("getrequests")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            var records = await _context.Requests!.ToListAsync();

            return Ok(records);
        }

        [HttpGet("getclients")]
        public async Task<ActionResult<IEnumerable<DocUploader.Shared.Models.Client>>> GetClients()
        {
            var records = await _context.Clients!.ToListAsync();

            return Ok(records);
        }
    }
}
