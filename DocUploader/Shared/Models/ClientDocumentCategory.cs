using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class ClientDocumentCategory
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int DocumentCategoryId { get; set; }
        public DocumentCategory DocumentCategory { get; set; }
    }
}
