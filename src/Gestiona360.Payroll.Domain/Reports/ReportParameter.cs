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
    /// Parámetros dinámicos para cada reporte.
    /// Tabla: ReportParameters
    /// </summary>
    [Table("ReportParameters")]
    public class ReportParameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ReportDefinitionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ParameterName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ParameterType { get; set; } = string.Empty; // Int, String, Date, Guid, Boolean, Decimal

        public bool IsRequired { get; set; } = true;

        [MaxLength(100)]
        public string DefaultValue { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DisplayLabel { get; set; } = string.Empty;

        // Relación con la definición del reporte
        [ForeignKey(nameof(ReportDefinitionId))]
        public virtual ReportDefinition ReportDefinition { get; set; } = null!;
    }

    /// <summary>
    /// Enumeración de tipos de parámetros (para uso en el frontend/backend)
    /// </summary>
    public enum ParameterTypeEnum
    {
        String,
        Int,
        Decimal,
        Date,
        DateTime,
        Boolean,
        Guid
    }
}
