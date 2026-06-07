namespace Gestiona360.Payroll.Application.Contracts.Requests
{
    public class CreatePayrollFrequencyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int DaysPerPeriod { get; set; }
        public int PeriodsPerYear { get; set; }
        public string? Description { get; set; }
    }

    // UpdatePayrollFrequencyRequest.cs
    public class UpdatePayrollFrequencyRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int DaysPerPeriod { get; set; }
        public int PeriodsPerYear { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
