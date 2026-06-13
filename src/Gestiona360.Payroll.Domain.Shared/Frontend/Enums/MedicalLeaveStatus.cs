using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{
    public enum MedicalLeaveStatus
    {
        Pending = 1,    // Pendiente de aprobación o de certificado INSS
        Approved = 2,   // Aprobada por RRHH / Jefe
        Paid = 3,       // Procesada en nómina
        Rejected = 4    // Rechazada (ej. documentación incompleta)
    }
}
