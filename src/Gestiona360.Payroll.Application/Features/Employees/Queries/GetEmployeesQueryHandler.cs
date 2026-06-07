using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, EmployeeSearchResultDto>
    {
        private readonly ApplicationDbContext _context;

        public GetEmployeesQueryHandler(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<EmployeeSearchResultDto> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            // Query base con includes para datos relacionados
            var query = _context.Employees
                .Include(e => e.Branch)
                .Include(e => e.CostCenter)
                .Include(e => e.JobGrade)
                    .ThenInclude(jg => jg!.JobPosition)
                .Include(e => e.ContractType)
                .AsQueryable();

            // 🔍 Filtro de búsqueda (nombre, cédula, código, RUC, INSS)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    e.Identification.ToLower().Contains(searchTerm) ||
                    e.Code.ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm) ||
                    e.NORUC.ToLower().Contains(searchTerm) ||
                    e.NOINSS.ToLower().Contains(searchTerm));
            }

            // 🏙️ Filtro por sucursal
            if (request.BranchId.HasValue)
                query = query.Where(e => e.BranchId == request.BranchId.Value);

            // 📄 Filtro por tipo de contrato
            if (request.ContractTypeId.HasValue)
                query = query.Where(e => e.ContractTypeId == request.ContractTypeId.Value);

            // ⚙️ Filtro por estado (activo/inactivo) - LEGACY
            if (!string.IsNullOrEmpty(request.Status))
            {
                var isActive = request.Status.ToLower() == "active";
                query = query.Where(e => e.IsActive == isActive);
            }

            // ✅ NUEVO: Filtro por EmploymentStatus (0=Active, 1=Suspended, 2=Terminated)
            if (request.EmploymentStatus.HasValue)
                query = query.Where(e => e.EmploymentStatus == (EmploymentStatus)request.EmploymentStatus.Value);

            // 👔 Filtro por puesto
            if (request.JobPositionId.HasValue)
                query = query.Where(e => e.JobGrade != null && e.JobGrade.JobPositionId == request.JobPositionId.Value);

            // ✅ NUEVO: Filtro por empleado de confianza
            if (request.IsTrustEmployee.HasValue)
                query = query.Where(e => e.IsTrustEmployee == request.IsTrustEmployee.Value);

            // ✅ NUEVO: Filtro por trabajador extranjero
            if (request.IsForeignWorker.HasValue)
            {
                if (request.IsForeignWorker.Value)
                    query = query.Where(e => !string.IsNullOrEmpty(e.Nationality));
                else
                    query = query.Where(e => string.IsNullOrEmpty(e.Nationality));
            }

            // ✅ NUEVO: Filtro por período de prueba
            if (request.IsOnProbation.HasValue)
            {
                var nowt = DateTime.UtcNow;
                if (request.IsOnProbation.Value)
                    query = query.Where(e => e.ProbationStartDate.HasValue &&
                                            e.ProbationEndDate.HasValue &&
                                            nowt >= e.ProbationStartDate.Value &&
                                            nowt <= e.ProbationEndDate.Value);
                else
                    query = query.Where(e => !e.ProbationStartDate.HasValue ||
                                            !e.ProbationEndDate.HasValue ||
                                            nowt < e.ProbationStartDate.Value ||
                                            nowt > e.ProbationEndDate.Value);
            }

            // ✅ NUEVO: Filtro por reingreso
            if (request.IsRehire.HasValue)
            {
                if (request.IsRehire.Value)
                    query = query.Where(e => e.PreviousEmployeeId.HasValue);
                else
                    query = query.Where(e => !e.PreviousEmployeeId.HasValue);
            }

            // 📊 Obtener empleados (proyección a DTO)
            var now = DateTime.UtcNow;
            var employees = await query
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new EmployeeListDto
                {
                    Id = e.Id,
                    Code = e.Code,
                    Identification = e.Identification,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Phone = e.Phone,
                    HireDate = e.HireDate,
                    IsActive = e.IsActive,
                    JobPositionName = e.JobGrade != null ? e.JobGrade.JobPosition.Name : "Sin puesto",
                    JobGradeName = e.JobGrade != null ? e.JobGrade.Name : "",
                    BranchName = e.Branch != null ? e.Branch.Name : "",
                    BranchCode = e.Branch != null ? e.Branch.Code : "",
                    ContractTypeName = e.ContractType != null ? e.ContractType.Name : "",

                    // ✅ NUEVOS CAMPOS
                    EmploymentStatus = e.EmploymentStatus,
                    //EmploymentStatusName = e.EmploymentStatus == EmploymentStatus.Active ? "Activo" :
                    //                      e.EmploymentStatus == EmploymentStatus.Suspended ? "Suspendido" :
                    //                      e.EmploymentStatus == EmploymentStatus.Terminated ? "Terminado" : "Desconocido",
                    IsTrustEmployee = e.IsTrustEmployee,
                    IsOnProbation = e.ProbationStartDate.HasValue &&
                                   e.ProbationEndDate.HasValue &&
                                   now >= e.ProbationStartDate.Value &&
                                   now <= e.ProbationEndDate.Value,
                    //IsForeignWorker = !string.IsNullOrEmpty(e.Nationality),
                    //IsRehire = e.PreviousEmployeeId.HasValue,
                    Nationality = e.Nationality,
                    ProbationEndDate = e.ProbationEndDate,

                    CostCenterId = e.CostCenterId,
                    CostCenterName = e.CostCenter != null ? e.CostCenter.Name : "",
                    CostCenterCode = e.CostCenter != null ? e.CostCenter.Code : ""
                })
                .ToListAsync(cancellationToken);

            // 📈 Calcular estadísticas (sobre TODOS los empleados, sin filtros)
            var allEmployees = _context.Employees;

            var stats = new EmployeeStatsDto
            {
                // Estadísticas existentes
                Total = await allEmployees.CountAsync(cancellationToken),
                Active = await allEmployees.CountAsync(e => e.IsActive, cancellationToken),
                Inactive = await allEmployees.CountAsync(e => !e.IsActive, cancellationToken),
                NewThisMonth = await allEmployees.CountAsync(e =>
                    e.HireDate.Month == now.Month && e.HireDate.Year == now.Year, cancellationToken),

                // ✅ NUEVAS ESTADÍSTICAS
                Suspended = await allEmployees.CountAsync(e =>
                    e.EmploymentStatus == EmploymentStatus.Suspended, cancellationToken),
                Terminated = await allEmployees.CountAsync(e =>
                    e.EmploymentStatus == EmploymentStatus.Terminated, cancellationToken),
                OnProbation = await allEmployees.CountAsync(e =>
                    e.ProbationStartDate.HasValue &&
                    e.ProbationEndDate.HasValue &&
                    now >= e.ProbationStartDate.Value &&
                    now <= e.ProbationEndDate.Value, cancellationToken),
                TrustEmployees = await allEmployees.CountAsync(e => e.IsTrustEmployee, cancellationToken),
                ForeignWorkers = await allEmployees.CountAsync(e => !string.IsNullOrEmpty(e.Nationality), cancellationToken),
                Rehires = await allEmployees.CountAsync(e => e.PreviousEmployeeId.HasValue, cancellationToken)
            };

            return new EmployeeSearchResultDto
            {
                Employees = employees,
                Stats = stats
            };
        }
    }
}
