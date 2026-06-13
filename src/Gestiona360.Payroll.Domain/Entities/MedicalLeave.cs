using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Domain.Entities
{
    [Table("MedicalLeaves")]
    //[Index(nameof(EmployeeId), nameof(StartDate), Name = "IX_MedicalLeaves_Employee_Date")]
    //[Index(nameof(CompanyId), Name = "IX_MedicalLeaves_Company")]
    //[Index(nameof(INSSCertificateNumber), IsUnique = true, Name = "IX_MedicalLeaves_INSSCert")]
    public class MedicalLeave : BaseEntityGuid
    {
        public Guid Id { get; set; }

        // VÍNCULO DIRECTO AL EMPLEADO (No a PersonalAction)
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;

        // VÍNCULO AL PERÍODO DE NÓMINA AFECTADO (Para trazabilidad de subsidios)
        public Guid? AffectedPayrollPeriodId { get; set; }
        public virtual PayrollPeriod? AffectedPayrollPeriod { get; set; }

        // DATOS DEL EVENTO MÉDICO
        public MedicalLeaveType LeaveType { get; set; } // CommonIllness, WorkAccident, OccupationalDisease, Maternity
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalDays { get; set; }

        // TRAZABILIDAD LEGAL Y FISCAL (INSS)
        [MaxLength(50)]
        public string? INSSCertificateNumber { get; set; } // Constancia de derecho / Incapacidad

        [MaxLength(50)]
        public string? HealthProviderName { get; set; } // Clínica/Hospital donde se atendió (Ley 618)

        // DATOS FINANCIEROS (Subsidios)
        public decimal BaseSalaryAtLeave { get; set; }
        public decimal INSSSubsidyAmount { get; set; }   // 60% (INSS)
        public decimal EmployerComplementAmount { get; set; } // 40% (Empleador, solo en Maternidad)

        public MedicalLeaveStatus Status { get; set; } // Pending, Approved, Paid, Rejected

        // RELACIÓN CON ASISTENCIA (Para pausar el reloj biométrico automáticamente)
        // Esto vincula la incapacidad con los registros de AttendanceRecord para justificar ausencias.
        public virtual ICollection<AttendanceRecord> AffectedAttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
