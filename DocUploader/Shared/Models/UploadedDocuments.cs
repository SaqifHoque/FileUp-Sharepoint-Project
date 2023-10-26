using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class UploadedDocuments
    {
        [Key]
        public int UploadedDocumentsId { get; set; }
        public int ClientRequestId { get; set; }
        public ClientsRequest? ClientsRequest { get; set; }  
        public string? DocumentPath { get; set; }   
    }
}
