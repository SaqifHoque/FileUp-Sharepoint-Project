using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class TableModel
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int RequestId { get; set; }
        public string? DocumentName { get; set; }
        public string? Status { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
