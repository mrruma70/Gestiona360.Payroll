using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands;

public class ImportMitrabSalariesCommandHandler : IRequestHandler<ImportMitrabSalariesCommand, Result>
{
    private readonly ApplicationDbContext _context;

    public ImportMitrabSalariesCommandHandler(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Result> Handle(ImportMitrabSalariesCommand request, CancellationToken cancellationToken)
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

            var csvRecords = new List<MitrabSalaryCsvRecord>();
            await foreach (var record in csv.GetRecordsAsync<MitrabSalaryCsvRecord>(cancellationToken))
            {
                csvRecords.Add(record);
            }

            if (!csvRecords.Any())
                return Result.Failure("El archivo CSV no contiene registros de salarios mínimos.");

            // Validar años
            var invalidYears = csvRecords.Where(r => r.Year <= 0).ToList();
            if (invalidYears.Any())
                return Result.Failure("El CSV debe contener una columna 'Year' con valores de año válidos (ej. 2024).");

            // ✅ Validar que todos tengan el mismo LegalReference
            var legalReferences = csvRecords.Select(r => r.LegalReference).Distinct().ToList();
            if (legalReferences.Count > 1)
                return Result.Failure("El archivo contiene múltiples fundamentos legales. Todos los sectores deben tener el mismo LegalReference.");

            var legalReference = legalReferences.First();
            if (string.IsNullOrWhiteSpace(legalReference))
                return Result.Failure("El campo LegalReference es obligatorio en todos los registros.");

            // Validar duplicados dentro del mismo archivo
            var duplicates = csvRecords
                .GroupBy(r => new { r.Year, r.SectorName })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicates.Any())
            {
                return Result.Failure($"El archivo contiene sectores duplicados para el mismo año: {string.Join(", ", duplicates.Select(d => $"{d.Year}-{d.SectorName}"))}");
            }

            var targetYear = csvRecords.First().Year;

            // Validar contra PayrollPeriods
            var periodsInYear = await _context.PayrollPeriods
                .Where(p => p.StartDate.Year == targetYear)
                .ToListAsync(cancellationToken);

            if (!periodsInYear.Any())
                return Result.Failure($"No existen períodos de nómina para el año {targetYear}. No se puede importar.");

            if (periodsInYear.Any(p => p.Status == "Closed"))
                return Result.Failure($"El año {targetYear} tiene períodos de nómina cerrados. No se puede modificar la matriz fiscal.");

            // Buscar o crear la cabecera (Schedule)
            var effectiveFrom = new DateOnly(targetYear, 1, 1);

            var schedule = await _context.MinimumWageSchedules
                .Where(s => s.EffectiveFrom.Year == targetYear && s.EffectiveTo == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (schedule == null)
            {
                // Crear nueva cabecera
                schedule = new MinimumWageSchedule
                {
                    EffectiveFrom = effectiveFrom,
                    EffectiveTo = null,
                    LegalReference = legalReference,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.MinimumWageSchedules.Add(schedule);
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

                if (request.Strategy == 1)
                {
                    // Estrategia 1: Borrar solo los detalles de esta cabecera
                    var currentSalaries = await _context.MinimumWages
                        .Where(s => s.ScheduleId == schedule.Id)
                        .ToListAsync(cancellationToken);
                    _context.MinimumWages.RemoveRange(currentSalaries);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            // Procesar cada registro
            foreach (var record in csvRecords)
            {
                if (string.IsNullOrWhiteSpace(record.SectorName))
                    continue;

                const int maxSectorLength = 100;
                if (record.SectorName.Length > maxSectorLength)
                    return Result.Failure($"El nombre del sector '{record.SectorName}' supera los {maxSectorLength} caracteres.");

                if (request.Strategy == 0)
                {
                    // Estrategia 0: Upsert (buscar por ScheduleId y Sector)
                    var existing = await _context.MinimumWages
                        .FirstOrDefaultAsync(s => s.ScheduleId == schedule.Id && s.Sector == record.SectorName,
                                           cancellationToken);

                    if (existing != null)
                    {
                        existing.MonthlyAmountNIO = record.MonthlyAmountNIO;
                        existing.MonthlyAmountUSD = record.MonthlyAmountUSD;
                        existing.UpdatedAt = DateTime.UtcNow;
                        _context.MinimumWages.Update(existing);
                        continue;
                    }
                }

                // Crear nuevo registro vinculado a la cabecera
                await _context.MinimumWages.AddAsync(new MinimumWage
                {
                    ScheduleId = schedule.Id,
                    Sector = record.SectorName,
                    MonthlyAmountNIO = record.MonthlyAmountNIO,
                    MonthlyAmountUSD = record.MonthlyAmountUSD,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateException dbEx)
        {
            await transaction.RollbackAsync(cancellationToken);
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            return Result.Failure($"Error de base de datos al guardar: {innerMessage}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            return Result.Failure($"Error procesando salarios mínimos MITRAB: {innerMessage}");
        }
    }
}

// Clase de mapeo CSV actualizada
public class MitrabSalaryCsvRecord
{
    public int Year { get; set; }
    public string SectorName { get; set; } = string.Empty; // ✅ Sin SectorCode
    public decimal MonthlyAmountNIO { get; set; }
    public decimal MonthlyAmountUSD { get; set; }
    public string LegalReference { get; set; } = string.Empty; // ✅ NUEVO
}