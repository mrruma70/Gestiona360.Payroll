using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{

    /// <summary>
    /// Estados del ciclo de vida de una Acción de Personal.
    /// </summary>
    public enum ActionStatus
    {
        /// <summary>Creada, pendiente de revisión o ejecución</summary>
        Pending = 1,

        /// <summary>Aprobada, lista para ejecutarse (fecha efectiva alcanzada)</summary>
        Approved = 2,

        /// <summary>Ejecutada: cambios aplicados a tablas maestras. INMUTABLE.</summary>
        Executed = 3,

        /// <summary>Rechazada con motivo documentado</summary>
        Rejected = 4
    }

}
