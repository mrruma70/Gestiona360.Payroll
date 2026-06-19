using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands;

public class ImportMitrabSalariesCommandHandler : IRequestHandler<ImportMitrabSalariesCommand, Result>
{
    private readonly IMinimumWageScheduleRepository _repository;
    private readonly MinimumWageScheduleDomainService _domainService;
    private readonly ICsvImportService _csvService;

    public ImportMitrabSalariesCommandHandler(
        IMinimumWageScheduleRepository repository,
        MinimumWageScheduleDomainService domainService,
        ICsvImportService csvService)
    {
        _repository = repository;
        _domainService = domainService;
        _csvService = csvService;
    }

    public async Task<Result> Handle(ImportMitrabSalariesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Leer CSV
            var csvRecords = await _csvService.ReadMitrabSalariesAsync(request.FileStream, cancellationToken);

            // 2. Validar datos del CSV
            _domainService.ValidateCsvData(csvRecords);

            var targetYear = csvRecords.First().Year;
            var legalReference = csvRecords.First().LegalReference;

            // 3. Validar que se pueda importar
            await _domainService.ValidateCanImportAsync(targetYear, cancellationToken);

            // 4. Buscar o crear schedule
            var schedule = await _repository.GetActiveScheduleByYearAsync(targetYear, cancellationToken);

            if (schedule == null)
            {
                // Crear nuevo schedule
                schedule = new MinimumWageSchedule
                {
                    EffectiveFrom = new DateOnly(targetYear, 1, 1),
                    EffectiveTo = null,
                    LegalReference = legalReference,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                schedule = await _repository.CreateScheduleAsync(schedule, cancellationToken);
            }
            else
            {
                // Actualizar LegalReference si cambió
                if (schedule.LegalReference != legalReference)
                {
                    schedule.LegalReference = legalReference;
                    schedule.UpdatedAt = DateTime.UtcNow;
                    await _repository.UpdateScheduleAsync(schedule, cancellationToken);
                }

                // Estrategia 1: Borrar todos los detalles y reemplazar
                if (request.Strategy == 1)
                {
                    await _repository.DeleteWagesByScheduleIdAsync(schedule.Id, cancellationToken);
                }
            }

            // 5. Procesar cada registro según la estrategia
            if (request.Strategy == 0)
            {
                // Estrategia 0: Upsert (actualizar existente o crear nuevo)
                await ProcessUpsertStrategyAsync(schedule.Id, csvRecords, cancellationToken);
            }
            else
            {
                // Estrategia 1: Insertar todos (ya se borraron los anteriores)
                var newWages = csvRecords.Select(r => new MinimumWage
                {
                    ScheduleId = schedule.Id,
                    Sector = r.SectorName,
                    MonthlyAmountNIO = r.MonthlyAmountNIO,
                    MonthlyAmountUSD = r.MonthlyAmountUSD,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _repository.AddWagesAsync(newWages, cancellationToken);
            }

            return Result.Success();
        }
        catch (BusinessRuleViolationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            return Result.Failure($"Error de base de datos al guardar: {innerMessage}");
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            return Result.Failure($"Error procesando salarios mínimos MITRAB: {innerMessage}");
        }
    }

    /// <summary>
    /// Procesa la estrategia Upsert (0): actualiza existentes o crea nuevos.
    /// </summary>
    private async Task ProcessUpsertStrategyAsync(
        int scheduleId,
        List<MitrabSalaryCsvRecord> records,
        CancellationToken ct)
    {
        foreach (var record in records)
        {
            var existing = await _repository.GetByScheduleAndSectorAsync(scheduleId, record.SectorName, ct);

            if (existing != null)
            {
                // Actualizar existente
                existing.MonthlyAmountNIO = record.MonthlyAmountNIO;
                existing.MonthlyAmountUSD = record.MonthlyAmountUSD;
                existing.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateWageAsync(existing, ct);
            }
            else
            {
                // Crear nuevo
                var newWage = new MinimumWage
                {
                    ScheduleId = scheduleId,
                    Sector = record.SectorName,
                    MonthlyAmountNIO = record.MonthlyAmountNIO,
                    MonthlyAmountUSD = record.MonthlyAmountUSD,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _repository.AddWageAsync(newWage, ct);
            }
        }
    }
}