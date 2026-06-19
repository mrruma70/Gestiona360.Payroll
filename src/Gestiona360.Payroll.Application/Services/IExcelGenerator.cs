using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Services
{

    public interface IExcelGenerator
    {
        byte[] GenerateEmployeeExport(IEnumerable<EmployeeExportDto> employees);

    }
}
