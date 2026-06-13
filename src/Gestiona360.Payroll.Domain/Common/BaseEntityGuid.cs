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

        public Guid CreatedBy { get; set; }
    
        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Timestamp]
        public byte[] RowVersion { get; set; } // Concurrencia optimista

        public bool IsActive { get; set; }
    }
}
