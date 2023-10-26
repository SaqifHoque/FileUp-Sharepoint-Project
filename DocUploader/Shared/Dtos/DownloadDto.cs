using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Dtos
{
    public class DownloadDto
    {
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public DateTime Time { get; set; }
    }
}
