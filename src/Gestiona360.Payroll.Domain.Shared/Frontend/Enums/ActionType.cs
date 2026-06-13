using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{
    /// <summary>
    /// Tipos de acciones de personal soportadas por el sistema.
    /// Cada tipo define qué campos fuertemente tipados deben llenarse en PersonalAction.
    /// </summary>
    public enum ActionType
    {
        /// <summary>Cambio de salario base. Afecta: Employees.BaseSalary</summary>
        SalaryChange = 1,

        /// <summary>Cambio de puesto/nivel. Afecta: Employees.JobGradeId</summary>
        PositionChange = 2,

        /// <summary>Cambio de tipo de contrato. Afecta: Employees.ContractTypeId</summary>
        ContractChange = 3,

        /// <summary>Cambio de turno asignado. Afecta: EmployeeShiftAssignments</summary>
        ShiftChange = 4,

        /// <summary>Cambio de centro de costo/sucursal. Afecta: Employees.CostCenterId</summary>
        CostCenterChange = 5,

        /// <summary>Suspensión individual (Art. 38 Ley 185)</summary>
        Suspension = 6,

        /// <summary>Suspensión colectiva autorizada por MITRAB</summary>
        MassSuspension = 7,

        /// <summary>Reincorporación post-suspensión</summary>
        Reinstatement = 8,

        /// <summary>Terminación individual de contrato</summary>
        Termination = 9,

        /// <summary>Terminación masiva (Quiebra/Cierre de empresa)</summary>
        MassTermination = 10,

        HealthProviderChange = 11,
        BankAccountChange = 12
    }
}
