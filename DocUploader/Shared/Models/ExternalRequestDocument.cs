using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class ExternalRequestDocument
    {
        [Key]
        public int DocumentIdInternal { get; set; }
        [Required]
        public string? DocumentsPath { get; set; }
        [Required]
        public string? DocumentsName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [Required]
        public int ExternalRequestId { get; set; }
        public virtual ExternalRequest? Request { get; set; }
    }
}
