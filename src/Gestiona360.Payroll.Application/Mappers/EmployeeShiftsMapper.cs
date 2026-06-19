using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Mappers
{
    public static class EmployeeShiftsMapper
    {
        public static EmployeeShiftsResultDto ToResultDto(EmployeeShiftsData data)
        {
            return new EmployeeShiftsResultDto
            {
                EmployeeName = data.EmployeeName,
                CurrentShift = data.CurrentShift != null ? ToCurrentShiftDto(data.CurrentShift) : null,
                ShiftHistory = data.ShiftHistory.Select(ToHistoryDto).ToList()
            };
        }

        private static EmployeeCurrentShiftDto ToCurrentShiftDto(EmployeeCurrentShiftInfo info)
        {
            return new EmployeeCurrentShiftDto
            {
                ShiftId = info.ShiftId,
                ShiftName = info.ShiftName,
                ShiftType = info.ShiftType,
                IsNightShift = info.IsNightShift,
                StartDate = info.StartDate,
                Schedules = info.Schedules.Select(ToScheduleDto).ToList()
            };
        }

        private static ShiftScheduleDto ToScheduleDto(ShiftScheduleInfo info)
        {
            return new ShiftScheduleDto
            {
                DayOfWeek = info.DayOfWeek,
                DayName = GetSpanishDayName(info.DayOfWeek),
                StartTime = info.StartTime,
                EndTime = info.EndTime,
                IsRestDay = info.IsRestDay
            };
        }

        private static EmployeeShiftHistoryDto ToHistoryDto(EmployeeShiftHistoryInfo info)
        {
            return new EmployeeShiftHistoryDto
            {
                AssignmentId = info.AssignmentId,
                ShiftName = info.ShiftName,
                StartDate = info.StartDate,
                EndDate = info.EndDate,
                Justification = info.Justification,
                ActionType = info.ActionType
            };
        }

        /// <summary>
        /// Convierte el número del día de la semana a nombre en español.
        /// Lógica de presentación que no pertenece al dominio ni a la infraestructura.
        /// </summary>
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
}
