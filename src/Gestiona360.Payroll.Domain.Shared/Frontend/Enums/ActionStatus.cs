using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{
    /// <summary>
    /// Estado del flujo de la acción de personal.
    /// </summary>
    public enum ActionStatus
    {
        Pending,    // Creada, pendiente de revisión/ejecución
        Approved,   // Aprobada, lista para ejecutarse en la fecha efectiva
        Executed,   // Ejecutada (INMUTABLE, ya afectó las tablas maestras)
        Rejected    // Rechazada con motivo
    }

}
