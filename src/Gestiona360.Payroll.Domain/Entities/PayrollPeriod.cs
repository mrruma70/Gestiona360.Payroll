using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class PayrollPeriod : BaseEntityGuid
    {
        public Guid PayrollGroupId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PeriodNumber { get; set; }             // ej. Semana 12

        [Required, MaxLength(20)]
        public string Status { get; set; }                // Open / PreClosed / Closed / Audited

        public Guid? ClosedBy { get; set; }               // UserId (Guid)
        [MaxLength(128)]
        public string ClosedHash { get; set; }            // SHA-256

        public virtual PayrollGroup PayrollGroup { get; set; }
        public virtual ICollection<PayrollRecord> PayrollRecords { get; set; }
        public virtual MonthlyClosing MonthlyClosing { get; set; }
    }
}
