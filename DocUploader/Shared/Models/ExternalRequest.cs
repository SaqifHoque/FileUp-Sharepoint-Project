using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class ExternalRequest
    {
        [Key]
        public int ExternalRequestId { get; set; }
        [Required]
        public int RequestIdInternal { get; set; }
        [Required]
        public string? RequestName { get; set; }
        public virtual ICollection<ExternalRequestDocument>? RequestDocuments { get; set; }
    }
}
