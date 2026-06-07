using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GetEmployeeShiftsQueryHandler : IRequestHandler<GetEmployeeShiftsQuery, EmployeeShiftsResultDto>
{
    private readonly ApplicationDbContext _context;

    public GetEmployeeShiftsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeShiftsResultDto> Handle(GetEmployeeShiftsQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new KeyNotFoundException("Empleado no encontrado.");

        // 1. Obtener el turno ACTUAL (EndDate es null o está en el futuro)
        var currentAssignment = await _context.EmployeeShiftAssignments
            .Where(a => a.EmployeeId == request.EmployeeId && (a.EndDate == null || a.EndDate >= DateTime.UtcNow))
            .Include(a => a.Shift).ThenInclude(s => s.Schedules)
            .OrderByDescending(a => a.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        EmployeeCurrentShiftDto? currentShiftDto = null;
        if (currentAssignment?.Shift != null)
        {
            currentShiftDto = new EmployeeCurrentShiftDto
            {
                ShiftId = currentAssignment.Shift.Id,
                ShiftName = currentAssignment.Shift.Name,
                ShiftType = currentAssignment.Shift.ShiftType,
                IsNightShift = currentAssignment.Shift.IsNightShift,
                StartDate = currentAssignment.StartDate,
                Schedules = currentAssignment.Shift.Schedules
                    .OrderBy(s => s.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)s.DayOfWeek)
                    .Select(s => new ShiftScheduleDto
                    {
                        DayOfWeek = (int)s.DayOfWeek,
                        DayName = GetSpanishDayName((int)s.DayOfWeek),
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        IsRestDay = s.IsRestDay
                    }).ToList()
            };
        }

        // 2. Obtener el historial de turnos
        var history = await _context.EmployeeShiftAssignments
            .Where(a => a.EmployeeId == request.EmployeeId)
            .Include(a => a.Shift)
            .Include(a => a.PersonalAction)
            .OrderByDescending(a => a.StartDate)
            .Select(a => new EmployeeShiftHistoryDto
            {
                AssignmentId = a.Id,
                ShiftName = a.Shift != null ? a.Shift.Name : "Turno Desconocido",
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Justification = a.Justification,
                // ✅ CORREGIDO: Convertir enum a string
                ActionType = a.PersonalAction != null ? a.PersonalAction.ActionType.ToString() : "Asignación Inicial"
            })
            .ToListAsync(cancellationToken);

        return new EmployeeShiftsResultDto
        {
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            CurrentShift = currentShiftDto,
            ShiftHistory = history
        };
    }

    private static string GetSpanishDayName(int dayOfWeek) => dayOfWeek switch
    {
        0 => "Domingo",
        1 => "Lunes",
        2 => "Martes",
        3 => "Miércoles",
        4 => "Jueves",
        5 => "Viernes",
        6 => "Sábado",
        _ => "Desconocido"
    };
}

