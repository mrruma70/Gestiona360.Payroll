using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Reports
{
    /// <summary>
    /// Catálogo de reportes configurables.
    /// Tabla: ReportDefinitions
    /// </summary>
    [Table("ReportDefinitions")]
    public class ReportDefinition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string QueryText { get; set; } = string.Empty;

        public bool IsStoredProcedure { get; set; } = false;

        [Required]
        [MaxLength(100)]
        public string AllowedFormats { get; set; } = "Excel,CSV,XML";

        [MaxLength(50)]
        public string RequiredRole { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación uno a muchos con los parámetros
        public virtual ICollection<ReportParameter> Parameters { get; set; } = new List<ReportParameter>();
    }
}
