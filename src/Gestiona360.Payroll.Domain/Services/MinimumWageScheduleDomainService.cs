using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Services
{
    public class MinimumWageScheduleDomainService
    {
        private readonly IMinimumWageScheduleRepository _repository;

        public MinimumWageScheduleDomainService(IMinimumWageScheduleRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Valida los datos del CSV antes de procesar.
        /// </summary>
        public void ValidateCsvData(List<MitrabSalaryCsvRecord> records)
        {
            if (!records.Any())
                throw new BusinessRuleViolationException("El archivo CSV no contiene registros de salarios mínimos.");

            // Validar años válidos
            var invalidYears = records.Where(r => r.Year <= 0).ToList();
            if (invalidYears.Any())
                throw new BusinessRuleViolationException("El CSV debe contener una columna 'Year' con valores de año válidos (ej. 2024).");

            // Validar LegalReference único
            var legalReferences = records.Select(r => r.LegalReference).Distinct().ToList();
            if (legalReferences.Count > 1)
                throw new BusinessRuleViolationException("El archivo contiene múltiples fundamentos legales. Todos los sectores deben tener el mismo LegalReference.");

            var legalReference = legalReferences.First();
            if (string.IsNullOrWhiteSpace(legalReference))
                throw new BusinessRuleViolationException("El campo LegalReference es obligatorio en todos los registros.");

            // Validar duplicados dentro del mismo archivo
            var duplicates = records
                .GroupBy(r => new { r.Year, r.SectorName })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                var duplicateList = string.Join(", ", duplicates.Select(d => $"{d.Year}-{d.SectorName}"));
                throw new BusinessRuleViolationException($"El archivo contiene sectores duplicados para el mismo año: {duplicateList}");
            }

            // Validar longitud de sector y que no esté vacío
            const int maxSectorLength = 100;
            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.SectorName))
                    throw new BusinessRuleViolationException("El nombre del sector no puede estar vacío.");

                if (record.SectorName.Length > maxSectorLength)
                    throw new BusinessRuleViolationException($"El nombre del sector '{record.SectorName}' supera los {maxSectorLength} caracteres.");

                if (record.MonthlyAmountNIO < 0)
                    throw new BusinessRuleViolationException($"El monto mensual en NIO ({record.MonthlyAmountNIO}) no puede ser negativo para el sector '{record.SectorName}'.");

                if (record.MonthlyAmountUSD < 0)
                    throw new BusinessRuleViolationException($"El monto mensual en USD ({record.MonthlyAmountUSD}) no puede ser negativo para el sector '{record.SectorName}'.");
            }
        }

        /// <summary>
        /// Valida que se pueda importar la matriz de salarios para el año especificado.
        /// </summary>
        public async Task ValidateCanImportAsync(int year, CancellationToken ct)
        {
            var hasPeriods = await _repository.HasPayrollPeriodsForYearAsync(year, ct);
            if (!hasPeriods)
                throw new BusinessRuleViolationException($"No existen períodos de nómina para el año {year}. No se puede importar.");

            var hasClosedPeriods = await _repository.HasClosedPayrollPeriodsForYearAsync(year, ct);
            if (hasClosedPeriods)
                throw new BusinessRuleViolationException($"El año {year} tiene períodos de nómina cerrados. No se puede modificar la matriz fiscal.");
        }
    }
}
