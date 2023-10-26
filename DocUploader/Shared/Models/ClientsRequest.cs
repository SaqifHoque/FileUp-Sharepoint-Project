using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class ClientsRequest
    {
        [Key]
        public int ClientRequestId { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public int RequestId { get; set; }
        public Request? Request { get; set; }
        public DateTime CreateDate { get; set; }

        public ICollection<UploadedDocuments>? UploadedDocuments { get; set; }   
        public ICollection<RequestDocuments>? RequestDocuments { get; set; }   

    }
}
