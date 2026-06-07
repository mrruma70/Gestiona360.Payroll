using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend.Enums
{
    /// <summary>
    /// Causales legales de suspensión (Art. 38 Ley 185).
    /// </summary>
    public enum SuspensionType
    {
        MedicalLeave,             // Enfermedad común o accidente no profesional
        OccupationalRisk,         // Accidente o enfermedad profesional
        LegalDisciplinary,        // Detención preventiva o medida disciplinaria
        MutualConsent,            // Mutuo consentimiento individual
        CivicDuty,                // Elección para cargo estatal
        MassForceMajeure          // Suspensión colectiva por fuerza mayor (MITRAB)
    }
}
