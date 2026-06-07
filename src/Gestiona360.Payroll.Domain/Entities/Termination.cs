using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Domain.Entities
{


    // rescisión del contrato de trabajo (Finiquito)
    public class Termination : BaseEntityGuid
    {
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        // Causal legal obligatoria para el cálculo
        public TerminationType TerminationType { get; set; }

        // Determina si se calcula indemnización (Art. 45) o solo proporcionales
        public bool IsJustified { get; set; }

        // 💰 Cálculos del Finiquito (Generados por el motor al ejecutar la acción)
        public decimal AguinaldoProportional { get; set; }
        public decimal VacationsPending { get; set; }
        public decimal IndemnityAmount { get; set; }
        public decimal PreavisoAmount { get; set; }
        public decimal TotalDeductions { get; set; } // Incluye préstamos, embargos, etc.
        public decimal TotalNetPayment { get; set; }

        // 📄 Documentación Legal
        public string DocumentPDF { get; set; } = string.Empty; // Ruta del Acta de Finiquito
        public bool SignedByEmployee { get; set; }

        // 🔗 Vinculación con la acción de personal que la originó (Trazabilidad)
        public Guid? LinkedPersonalActionId { get; set; }
        public virtual PersonalAction? LinkedPersonalAction { get; set; }


    }
}
