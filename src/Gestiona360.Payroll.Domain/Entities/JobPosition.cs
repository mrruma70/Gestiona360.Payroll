using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class JobPosition : BaseEntityGuid
    {
        [Required, MaxLength(20)]
        public string Code { get; set; }            // SOLD, CONT

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }        // Operativo / Administrativo / Comercial

        public int? OccupationalRiskId { get; set; }
        public int? MinimumWageId { get; set; }      // FK a tabla de salarios mínimos (int)
  

        [ForeignKey(nameof(MinimumWageId))]
        public virtual MinimumWage? MinimumWage { get; set; }
        public string Description { get; set; }

        public bool IsTrustPosition { get; set; }   // Si el puesto es de confianza por defecto
        public bool RequiresLicense { get; set; }   // Ya existe en JobGrade, pero puede estar aquí también

        public virtual OccupationalRisk OccupationalRisk { get; set; }
        public virtual ICollection<JobGrade> Grades { get; set; }
    }
}
