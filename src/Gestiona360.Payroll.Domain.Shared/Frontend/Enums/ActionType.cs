using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{
    /// <summary>
    /// Tipos de acciones de personal permitidas.
    /// </summary>
    public enum ActionType
    {
        SalaryChange,         // Cambio de salario base
        PositionChange,       // Cambio de puesto/nivel
        ContractChange,       // Cambio de tipo de contrato
        ShiftChange,          // Cambio de turno
        CostCenterChange,     // Traslado de centro de costo/sucursal
        Suspension,           // Suspensión individual
        MassSuspension,       // Suspensión colectiva (requiere MITRAB)
        Reinstatement,        // Reincorporación post-suspensión
        Termination,          // Terminación individual
        MassTermination       // Terminación masiva (Quiebra/Cierre)
    }
}
