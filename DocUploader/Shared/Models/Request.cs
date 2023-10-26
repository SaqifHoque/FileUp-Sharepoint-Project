using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }
        public int RequestIdInternal { get; set; }
        public string? RequestName { get; set; }
        public ICollection<ClientsRequest>? ClientsRequests { get; set; }
        public ICollection<RequestDocuments>? RequestDocuments { get; set; }


    }
}
