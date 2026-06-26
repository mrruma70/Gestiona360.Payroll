using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO para Bank
    /// Catálogo de bancos para dispersión de nómina
    /// </summary>
    public class BankDto : CatalogItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string AchCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }

      
        [StringLength(50)]
        public string BankCode { get; set; } // Código del banco

        [Phone]
        public string ContactPhone { get; set; }

        [EmailAddress]
        public string ContactEmail { get; set; }

        public bool SupportsACH { get; set; } = true;
    }

}
