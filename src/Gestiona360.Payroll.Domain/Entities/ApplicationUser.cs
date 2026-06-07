using Microsoft.AspNetCore.Identity;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Cambia a Guid? para que coincida con Employee.Id
        public Guid? EmployeeId { get; set; }

        // Relación con Employee (si la necesitas)
        public virtual Employee Employee { get; set; }
    }
}