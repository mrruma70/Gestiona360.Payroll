using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class EmployeeBarcodeInfo
    {
        public string Code { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public string? ExistingBarcode { get; set; }
    }
}
