using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared
{
    public class ValidationErrorResponse
    {
        public string Title { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
