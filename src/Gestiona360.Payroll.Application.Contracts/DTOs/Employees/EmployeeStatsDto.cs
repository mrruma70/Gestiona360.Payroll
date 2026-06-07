using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeStatsDto
    {
        // Estadísticas existentes
        public int Total { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
        public int NewThisMonth { get; set; }

        // ✅ NUEVAS ESTADÍSTICAS
        public int Suspended { get; set; }
        public int Terminated { get; set; }
        public int OnProbation { get; set; }
        public int TrustEmployees { get; set; }
        public int ForeignWorkers { get; set; }
        public int Rehires { get; set; }
    }

}
