using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocUploader.Shared.Models
{
    public class ExternalClient
    {
        [Key]
        public int ExternalClientId { get; set; }
        [Required]
        public int ClientIdInternal { get; set; }
        [Required]
        public string? ClientName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [StringLength(4)]
        public string? Last4Digits { get; set; }


        [StringLength(255, MinimumLength = 12)]
        public string? Password { get; set; }
    }
}
