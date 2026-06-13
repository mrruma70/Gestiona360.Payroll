using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    public class PersonalActionDocumentDto
    {
        public Guid DocumentTypeId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
    }
}
