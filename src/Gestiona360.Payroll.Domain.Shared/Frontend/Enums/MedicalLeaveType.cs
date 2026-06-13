using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{
    public enum MedicalLeaveType
    {
        CommonIllness = 1,      // Enfermedad Común (INSS a partir del 4to día)
        OccupationalRisk = 2,   // Riesgo Profesional / Accidente de Trabajo (INSS desde el 1er día)
        Maternity = 3           // Maternidad (90 días: 60% INSS + 40% Empleador)
    }

}
