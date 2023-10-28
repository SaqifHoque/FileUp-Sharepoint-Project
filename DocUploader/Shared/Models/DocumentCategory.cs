using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class DocumentCategory
    {
        [Key]
        public int DocumentCategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<ClientDocumentCategory> ClientDocumentCategories { get; set; }
    }
}
