using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{
    /// <summary>
    /// Causales legales de terminación (Arts. 44-48 Ley 185 y Código de Comercio).
    /// </summary>
    public enum TerminationType
    {
        Resignation,              // Renuncia voluntaria (con o sin preaviso)
        UnfairDismissal,          // Despido injustificado (genera indemnización Art. 45)
        JustifiedDismissal,       // Despido justificado (Art. 48, sin indemnización)
        MutualAgreement,          // Mutuo acuerdo / Vencimiento de plazo fijo
        Bankruptcy,               // Quiebra o cierre de empresa (Crédito laboral privilegiado)
        Death,                    // Fallecimiento del empleado
        Probation                 // Terminación durante período de prueba
    }
}
