using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmtpManagement.Models.SmtpConnections
{
    public sealed class CreateModel
    {
        [Required]
        public string Host { get; set; }

        [Required]
        public string Name { get; set; }

        public int Port { get; set; }

        public bool UseSsl { get; set; }
    }
}
