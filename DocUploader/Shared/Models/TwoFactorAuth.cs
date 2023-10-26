using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class TwoFactorAuth
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}
