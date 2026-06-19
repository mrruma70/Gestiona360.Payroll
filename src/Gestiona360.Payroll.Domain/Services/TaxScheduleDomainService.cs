using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;

namespace Gestiona360.Payroll.Domain.Services
{
    public class TaxScheduleDomainService
    {
        private readonly ITaxScheduleRepository _repository;

        public TaxScheduleDomainService(ITaxScheduleRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Valida los datos del CSV antes de procesar.
        /// </summary>
        public void ValidateCsvData(List<DgiTaxBracketCsvRecord> records)
        {
            if (!records.Any())
                throw new BusinessRuleViolationException("El archivo CSV no contiene registros de tramos IR.");

            // Validar año único
            var distinctYears = records.Select(r => r.Year).Distinct().ToList();
            if (distinctYears.Count > 1)
                throw new BusinessRuleViolationException("El archivo contiene múltiples años. Cada importación debe ser para un solo año fiscal.");

            var targetYear = distinctYears.First();
            if (targetYear <= 0)
                throw new BusinessRuleViolationException("El año debe ser un número positivo.");

            // Validar LegalReference único
            var legalReferences = records.Select(r => r.LegalReference).Distinct().ToList();
            if (legalReferences.Count > 1)
                throw new BusinessRuleViolationException("El archivo contiene múltiples fundamentos legales. Todos los tramos deben tener el mismo LegalReference.");

            var legalReference = legalReferences.First();
            if (string.IsNullOrWhiteSpace(legalReference))
                throw new BusinessRuleViolationException("El campo LegalReference es obligatorio en todos los registros.");

            // Validar rangos y tasas
            foreach (var record in records)
            {
                if (record.FromAmount < 0)
                    throw new BusinessRuleViolationException($"El valor 'FromAmount' ({record.FromAmount}) no puede ser negativo.");
                if (record.ToAmount < 0)
                    throw new BusinessRuleViolationException($"El valor 'ToAmount' ({record.ToAmount}) no puede ser negativo.");
                if (record.ToAmount > 0 && record.ToAmount <= record.FromAmount)
                    throw new BusinessRuleViolationException($"El rango {record.FromAmount} - {record.ToAmount} es inválido (ToAmount debe ser mayor que FromAmount o 0 para el último tramo).");
                if (record.MarginalRate < 0 || record.MarginalRate > 1)
                    throw new BusinessRuleViolationException($"La tasa marginal {record.MarginalRate} debe estar entre 0 y 1.");
            }
        }

        /// <summary>
        /// Valida que se pueda importar la matriz fiscal para el año especificado.
        /// </summary>
        public async Task ValidateCanImportAsync(int year, CancellationToken ct)
        {
            var hasPeriods = await _repository.HasPayrollPeriodsForYearAsync(year, ct);
            if (!hasPeriods)
                throw new BusinessRuleViolationException($"No existen períodos de nómina para el año {year}. No se puede importar la matriz fiscal.");

            var hasClosedPeriods = await _repository.HasClosedPayrollPeriodsForYearAsync(year, ct);
            if (hasClosedPeriods)
                throw new BusinessRuleViolationException($"El año {year} tiene períodos de nómina cerrados. Cambiar la matriz fiscal podría afectar cálculos ya ejecutados. Operación cancelada.");
        }
    }

    // DTO para lectura CSV (puede ir en Domain o Application)
    public class DgiTaxBracketCsvRecord
    {
        public int Year { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public decimal FixedTax { get; set; }
        public decimal MarginalRate { get; set; }
        public string LegalReference { get; set; } = string.Empty;
    }
}
