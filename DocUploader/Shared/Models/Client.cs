using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }
        public int ClientIdInternal { get; set; }
        public string? ClientName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string? Last4Digits { get; set; }

        [StringLength(255, MinimumLength = 12)]
        public string? Password { get; set; }

        public ICollection<ClientsRequest>? ClientsRequests { get; set; }    
        public ICollection<UploadedDocuments>? UploadedDocuments { get; set; }
        public ICollection<ClientDocumentCategory> ClientDocumentCategories { get; set; }

    }
}
