using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    public class ActionConfigurationDto
    {
        public IEnumerable<CausalDto> Causals { get; set; } = new List<CausalDto>();
        public IEnumerable<DocumentRequirementDto> RequiredDocuments { get; set; } = new List<DocumentRequirementDto>();
    }
}
