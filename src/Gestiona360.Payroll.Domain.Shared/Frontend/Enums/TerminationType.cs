using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{
    /// <summary>
    /// Causales legales de terminación según Arts. 44-48 Ley 185 y Código de Comercio.
    /// </summary>
    public enum TerminationType
    {
        /// <summary>Renuncia voluntaria (con o sin preaviso)</summary>
        Resignation = 1,

        /// <summary>Despido injustificado (genera indemnización Art. 45)</summary>
        UnfairDismissal = 2,

        /// <summary>Despido justificado (Art. 48, sin indemnización)</summary>
        JustifiedDismissal = 3,

        /// <summary>Mutuo acuerdo / Vencimiento de plazo fijo</summary>
        MutualAgreement = 4,

        /// <summary>Quiebra o cierre de empresa (crédito laboral privilegiado)</summary>
        Bankruptcy = 5,

        /// <summary>Fallecimiento del empleado</summary>
        Death = 6,

        /// <summary>Terminación durante período de prueba</summary>
        Probation = 7
    }
}
