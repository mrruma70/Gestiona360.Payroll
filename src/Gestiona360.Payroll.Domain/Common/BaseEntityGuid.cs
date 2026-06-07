using System.ComponentModel.DataAnnotations;

namespace Gestiona360.Payroll.Domain.Common
{
    // Para entidades con clave Guid
    public abstract class BaseEntityGuid
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } // Concurrencia optimista

        public bool IsActive { get; set; }
    }
}
