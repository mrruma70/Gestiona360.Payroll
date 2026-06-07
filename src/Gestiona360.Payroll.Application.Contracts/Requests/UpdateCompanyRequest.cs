using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.Requests
{
    public class UpdateCompanyRequest
    {
        [Required] public Guid Id { get; set; } // ✅ Guid
        [Required] public string LegalName { get; set; } = string.Empty;
        [Required] public string CommercialName { get; set; } = string.Empty;
        [Required] public string TaxId { get; set; } = string.Empty;
        [Required] public string INSSEmployerCode { get; set; } = string.Empty;
        [Required] public string EconomicActivityCode { get; set; } = string.Empty;
        [Required] public string Phone { get; set; } = string.Empty;
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        [Required] public string City { get; set; } = string.Empty;
        [Required] public string Department { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        [Required] public string LegalRepresentative { get; set; } = string.Empty;
        [Required] public string LegalRepresentativeId { get; set; } = string.Empty;
        [Required] public string DefaultCurrency { get; set; } = "NIO";
        [Required] public int DefaultPayrollFrequencyId { get; set; }
        [Required] public bool DefaultIsZoneFranca { get; set; }

        public decimal TotalActiveEmployees { get; set; }
        public decimal INATECRate { get; set; }
    }
}
