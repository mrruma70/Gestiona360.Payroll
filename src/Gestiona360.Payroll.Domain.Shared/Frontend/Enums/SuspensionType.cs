using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{
    /// <summary>
    /// Causales legales de suspensión según Art. 38 Ley 185 (Nicaragua).
    /// </summary>
    public enum SuspensionType
    {
        /// <summary>Enfermedad común o accidente no profesional (INSS paga desde día 4)</summary>
        MedicalLeave = 1,

        /// <summary>Accidente o enfermedad profesional</summary>
        OccupationalRisk = 2,

        /// <summary>Detención preventiva o medida disciplinaria</summary>
        LegalDisciplinary = 3,

        /// <summary>Mutuo consentimiento individual</summary>
        MutualConsent = 4,

        /// <summary>Elección para cargo estatal (Art. 52 Constitución)</summary>
        CivicDuty = 5,

        /// <summary>Suspensión colectiva por fuerza mayor (requiere MITRAB)</summary>
        MassForceMajeure = 6
    }
}
