using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands;

public class ImportDgiTaxBracketCommandHandler : IRequestHandler<ImportDgiTaxBracketCommand, Result>
{
    private readonly ApplicationDbContext _context;

    public ImportDgiTaxBracketCommandHandler(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Result> Handle(ImportDgiTaxBracketCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            using var reader = new StreamReader(request.FileStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                PrepareHeaderForMatch = args => args.Header.ToLower()
            });

            var csvRecords = new List<DgiTaxBracketCsvRecord>();
            await foreach (var record in csv.GetRecordsAsync<DgiTaxBracketCsvRecord>(cancellationToken))
            {
                csvRecords.Add(record);
            }

            if (!csvRecords.Any())
                return Result.Failure("El archivo CSV no contiene registros de tramos IR.");

            // Validar que todos los registros tengan el mismo año
            var distinctYears = csvRecords.Select(r => r.Year).Distinct().ToList();
            if (distinctYears.Count > 1)
                return Result.Failure("El archivo contiene múltiples años. Cada importación debe ser para un solo año fiscal.");

            var targetYear = distinctYears.First();
            if (targetYear <= 0)
                return Result.Failure("El año debe ser un número positivo.");

            // ✅ Validar que todos tengan el mismo LegalReference
            var legalReferences = csvRecords.Select(r => r.LegalReference).Distinct().ToList();
            if (legalReferences.Count > 1)
                return Result.Failure("El archivo contiene múltiples fundamentos legales. Todos los tramos deben tener el mismo LegalReference.");

            var legalReference = legalReferences.First();
            if (string.IsNullOrWhiteSpace(legalReference))
                return Result.Failure("El campo LegalReference es obligatorio en todos los registros.");

            // Validar contra PayrollPeriods
            var periodsInYear = await _context.PayrollPeriods
                .Where(p => p.StartDate.Year == targetYear)
                .ToListAsync(cancellationToken);

            if (!periodsInYear.Any())
                return Result.Failure($"No existen períodos de nómina para el año {targetYear}. No se puede importar la matriz fiscal.");

            if (periodsInYear.Any(p => p.Status == "Closed"))
                return Result.Failure($"El año {targetYear} tiene períodos de nómina cerrados. Cambiar la matriz fiscal podría afectar cálculos ya ejecutados. Operación cancelada.");

            // Validar rangos y tasas
            foreach (var record in csvRecords)
            {
                if (record.FromAmount < 0)
                    return Result.Failure($"El valor 'FromAmount' ({record.FromAmount}) no puede ser negativo.");
                if (record.ToAmount < 0)
                    return Result.Failure($"El valor 'ToAmount' ({record.ToAmount}) no puede ser negativo.");
                if (record.ToAmount > 0 && record.ToAmount <= record.FromAmount)
                    return Result.Failure($"El rango {record.FromAmount} - {record.ToAmount} es inválido (ToAmount debe ser mayor que FromAmount o 0 para el último tramo).");
                if (record.MarginalRate < 0 || record.MarginalRate > 1)
                    return Result.Failure($"La tasa marginal {record.MarginalRate} debe estar entre 0 y 1.");
            }

            // Buscar o crear la cabecera (Schedule)
            var effectiveFrom = new DateOnly(targetYear, 1, 1);

            var schedule = await _context.IrTaxSchedules
                .Where(s => s.EffectiveFrom.Year == targetYear && s.EffectiveTo == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (schedule == null)
            {
                // Crear nueva cabecera
                schedule = new IrTaxSchedule
                {
                    EffectiveFrom = effectiveFrom,
                    EffectiveTo = null,
                    LegalReference = legalReference,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.IrTaxSchedules.Add(schedule);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                // Actualizar LegalReference si cambió
                if (schedule.LegalReference != legalReference)
                {
                    schedule.LegalReference = legalReference;
                    schedule.UpdatedAt = DateTime.UtcNow;
                }

                // Eliminar tramos existentes de esta cabecera
                var existingBrackets = await _context.IR_TaxBrackets
                    .Where(b => b.ScheduleId == schedule.Id)
                    .ToListAsync(cancellationToken);
                _context.IR_TaxBrackets.RemoveRange(existingBrackets);
            }

            // Crear nuevos tramos vinculados a la cabecera
            var newBrackets = csvRecords.Select(r => new IR_TaxBracket
            {
                ScheduleId = schedule.Id,
                FromAmount = r.FromAmount,
                ToAmount = r.ToAmount > 0 ? r.ToAmount : (decimal?)null, // ✅ 0 se convierte a NULL
                FixedTax = r.FixedTax,
                MarginalRate = r.MarginalRate * 100m, // 0.15 -> 15.00
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.IR_TaxBrackets.AddRangeAsync(newBrackets, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (DbUpdateException dbEx)
        {
            await transaction.RollbackAsync(cancellationToken);
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            return Result.Failure($"Error de base de datos: {innerMessage}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            return Result.Failure($"Error procesando tramos DGI IR: {innerMessage}");
        }
    }
}

// Clase de mapeo CSV actualizada
public class DgiTaxBracketCsvRecord
{
    public int Year { get; set; }
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal FixedTax { get; set; }
    public decimal MarginalRate { get; set; }
    public string LegalReference { get; set; } = string.Empty; // ✅ NUEVO
}