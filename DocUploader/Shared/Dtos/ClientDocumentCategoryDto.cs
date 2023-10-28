using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Dtos
{
    public class ClientDocumentCategoryDto
    {
        public int Id { get; set; }
        public string DocumentCategoryName {  get; set; }
        public string ClientName { get; set; }
    }
}
