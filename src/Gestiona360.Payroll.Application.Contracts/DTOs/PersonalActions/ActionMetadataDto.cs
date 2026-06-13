using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    public class ActionMetadataDto
    {
        public List<string> AvailableCausals { get; set; } = new();
        public List<string> RequiredDocuments { get; set; } = new();
        public decimal? MinSalary { get; set; }
    }
}
