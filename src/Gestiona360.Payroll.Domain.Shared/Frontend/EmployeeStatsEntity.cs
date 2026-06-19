using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Entidad de dominio para estadísticas de empleados.
    /// No se persiste en BD, solo se usa para transporte de datos.
    /// </summary>
    public class EmployeeStatsEntity
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
        public int NewThisMonth { get; set; }
        public int Suspended { get; set; }
        public int Terminated { get; set; }
        public int OnProbation { get; set; }
        public int TrustEmployees { get; set; }
        public int ForeignWorkers { get; set; }
        public int Rehires { get; set; }
    }
}
