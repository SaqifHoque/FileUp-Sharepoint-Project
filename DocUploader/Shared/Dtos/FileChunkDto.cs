using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Dtos
{
    public class FileChunkDto
    {
        public string FileName { get; set; } = "";
        public long Offset { get; set; }
        public byte[]? Data { get; set; }
        public bool FirstChunk = false;
        public bool LastChunk { get; set; }
        public string? To { get; set; } = null;
        public int ClientId { get; set; }
        public int RequestId { get; set; }
    }
}
