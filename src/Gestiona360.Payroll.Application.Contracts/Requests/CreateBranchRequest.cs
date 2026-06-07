using System.ComponentModel.DataAnnotations;

namespace Gestiona360.Payroll.Application.Contracts.Requests
{
    public class CreateBranchRequest
    {
        [Required]
        [RegularExpression(@"^SUC-\d{3}$", ErrorMessage = "El código debe tener formato SUC-XXX")]
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public Guid? ManagerEmployeeId { get; set; }
        public Guid? DefaultCostCenterId { get; set; }
        public bool IsZoneFranca { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
