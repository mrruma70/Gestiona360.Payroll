using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class Company : BaseEntityGuid
    {
        [Required, MaxLength(100)]
        public string LegalName { get; set; }

        [Required, MaxLength(100)]
        public string CommercialName { get; set; }

        [Required, MaxLength(20)]
        public string TaxId { get; set; }                 // RUC

        [Required, MaxLength(20)]
        public string INSSEmployerCode { get; set; }

        [Required, MaxLength(10)]
        public string EconomicActivityCode { get; set; }  // CIIU

        public int TotalActiveEmployees { get; set; }

        [Phone, MaxLength(20)]
        public string Phone { get; set; }

        [EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string Department { get; set; }

        [MaxLength(500)]
        public string LogoUrl { get; set; }

        [Required, MaxLength(100)]
        public string LegalRepresentative { get; set; }

        [Required, MaxLength(20)]
        public string LegalRepresentativeId { get; set; }

        [Required, MaxLength(3)]
        public string DefaultCurrency { get; set; } = "NIO"; // NIO o USD

        public int DefaultPayrollFrequencyId { get; set; }   // FK a PayrollFrequency (int)
        public bool DefaultIsZoneFranca { get; set; }
        public bool IsActive { get; set; } = true;

  
        // Navigation properties
        public virtual PayrollFrequency DefaultPayrollFrequency { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<HealthProvider> HealthProviders { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        //public virtual ICollection<PayrollGroup> PayrollGroups { get; set; }
        public virtual ICollection<ZFBenefit> ZFBenefits { get; set; }
    }
}
