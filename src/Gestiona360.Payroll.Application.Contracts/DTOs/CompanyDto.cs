using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{


    public class CompanyDto
    {
        public Guid Id { get; set; } // ✅ Guid
        public string LegalName { get; set; } = string.Empty;
        public string CommercialName { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string INSSEmployerCode { get; set; } = string.Empty;
        public string EconomicActivityCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string LegalRepresentative { get; set; } = string.Empty;
        public string LegalRepresentativeId { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "NIO";
        public int DefaultPayrollFrequencyId { get; set; }
        public bool DefaultIsZoneFranca { get; set; }

        public decimal TotalActiveEmployees { get; set; }


    }
}
