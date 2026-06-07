using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class YearEndClosing : BaseEntityGuid
    {
        public int Year { get; set; }
        [MaxLength(20)]
        public string Status { get; set; } = "Closed";
        public Guid ExecutedBy { get; set; }
        [MaxLength(128)]
        public string HashSHA256 { get; set; }
        public bool CertificatesGenerated { get; set; }
    }
}
