using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Dtos
{
    public class UploadFileRequestModel
    {
        public string SiteId { get; set; }
        public string DriveId { get; set; }
        public string FolderId { get; set; }
        public string FileLocalPath { get; set; }
        public FileInfo FileInfo { get; set; }
    }
}
