using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class RequestDocuments
    {
        [Key]
        public int RequestDocumentId { get; set; }
        public int RequestId { get; set; }
        public Request? Request { get; set; }
        public string? DocumentName { get; set; }

    }
}
