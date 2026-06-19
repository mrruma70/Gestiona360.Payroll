using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands;

public class ImportDgiTaxBracketCommandHandler : IRequestHandler<ImportDgiTaxBracketCommand, Result>
{
    private readonly ITaxScheduleRepository _repository;
    private readonly TaxScheduleDomainService _domainService;
    private readonly ICsvImportService _csvService;
    private readonly IUnitOfWork _unitOfWork;

    public ImportDgiTaxBracketCommandHandler(
        ITaxScheduleRepository repository,
        TaxScheduleDomainService domainService,
        ICsvImportService csvService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _domainService = domainService;
        _csvService = csvService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ImportDgiTaxBracketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Leer CSV
            var csvRecords = await _csvService.ReadDgiTaxBracketsAsync(request.FileStream, cancellationToken);

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
                schedule = new IrTaxSchedule
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

                // Eliminar tramos existentes
                await _repository.DeleteBracketsByScheduleIdAsync(schedule.Id, cancellationToken);
            }

            // 5. Crear nuevos tramos
            var newBrackets = csvRecords.Select(r => new IR_TaxBracket
            {
                ScheduleId = schedule.Id,
                FromAmount = r.FromAmount,
                ToAmount = r.ToAmount > 0 ? r.ToAmount : (decimal?)null,
                FixedTax = r.FixedTax,
                MarginalRate = r.MarginalRate * 100m, // 0.15 -> 15.00
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _repository.AddBracketsAsync(newBrackets, cancellationToken);

            return Result.Success();
        }
        catch (BusinessRuleViolationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            return Result.Failure($"Error de base de datos: {innerMessage}");
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            return Result.Failure($"Error procesando tramos DGI IR: {innerMessage}");
        }
    }
}