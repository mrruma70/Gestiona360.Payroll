using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Config.Queries;

public class GetGlobalConfigQueryHandler : IRequestHandler<GetGlobalConfigQuery, GlobalConfigDto>
{
    private readonly ApplicationDbContext _context;

    public GetGlobalConfigQueryHandler(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<GlobalConfigDto> Handle(GetGlobalConfigQuery request, CancellationToken cancellationToken)
    {
        // =================================================================
        // 1. AÑOS DISPONIBLES
        // =================================================================
        var availableINSSYears = await _context.INSSConfigs
            .Select(c => c.EffectiveFrom.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(cancellationToken);

        var availableIRYears = await _context.IrTaxSchedules
            .Select(s => s.EffectiveFrom.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(cancellationToken);

        var availableMitrabYears = await _context.MinimumWageSchedules
            .Select(s => s.EffectiveFrom.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(cancellationToken);

        var availableINATECYears = await _context.INATECConfigs
            .Select(c => c.EffectiveFrom.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(cancellationToken);

        // =================================================================
        // 2. DETERMINAR AÑOS OBJETIVO (✅ CORRECCIÓN: Tratar 0 como null)
        // =================================================================
        int? yINSS = request.YearINSS == 0 ? null : request.YearINSS;
        int? yIR = request.YearIR == 0 ? null : request.YearIR;
        int? yMitrab = request.YearMitrab == 0 ? null : request.YearMitrab;
        int? yINATEC = request.YearINATEC == 0 ? null : request.YearINATEC;

        int targetINSSYear = yINSS ?? availableINSSYears.FirstOrDefault();
        int targetIRYear = yIR ?? availableIRYears.FirstOrDefault();
        int targetMitrabYear = yMitrab ?? availableMitrabYears.FirstOrDefault();
        int targetINATECYear = yINATEC ?? availableINATECYears.FirstOrDefault();

        // =================================================================
        // 3. INSS CONFIG
        // =================================================================
        var inssConfig = new INSSConfigDto();
        if (targetINSSYear != 0)
        {
            var inssEntity = await _context.INSSConfigs
                .Where(c => c.EffectiveFrom.Year == targetINSSYear && c.IsActive)
                .OrderByDescending(c => c.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);

            if (inssEntity != null)
            {
                inssConfig = new INSSConfigDto
                {
                    Year = inssEntity.EffectiveFrom.Year,
                    RateWorker = inssEntity.RateWorker,
                    RateEmployerSmall = inssEntity.RateEmployerSmall,
                    RateEmployerLarge = inssEntity.RateEmployerLarge,
                    MaxSalaryCap = inssEntity.MaxSalaryCap,
                    LegalReference = inssEntity.LegalReference ?? "",
                    EffectiveFrom = inssEntity.EffectiveFrom,
                    EffectiveTo = inssEntity.EffectiveTo
                };
            }
        }

        // =================================================================
        // 4. INATEC CONFIG
        // =================================================================
        var inatecConfig = new INATECConfigDto();
        if (targetINATECYear != 0)
        {
            var inatecEntity = await _context.INATECConfigs
                .Where(c => c.EffectiveFrom.Year == targetINATECYear && c.IsActive)
                .OrderByDescending(c => c.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);

            if (inatecEntity != null)
            {
                inatecConfig = new INATECConfigDto
                {
                    Year = inatecEntity.EffectiveFrom.Year,
                    Rate = inatecEntity.Rate,
                    Exceptions = inatecEntity.Exceptions ?? "",
                    LegalReference = inatecEntity.LegalReference ?? "",
                    EffectiveFrom = inatecEntity.EffectiveFrom,
                    EffectiveTo = inatecEntity.EffectiveTo
                };
            }
        }

        // =================================================================
        // 5. IR TAX BRACKETS
        // =================================================================
        var irBrackets = new List<IR_TaxBracketDto>();
        if (targetIRYear != 0)
        {
            var irSchedule = await _context.IrTaxSchedules
                .Where(s => s.EffectiveFrom.Year == targetIRYear && s.IsActive)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);

            if (irSchedule != null)
            {
                var brackets = await _context.IR_TaxBrackets
                    .Where(b => b.ScheduleId == irSchedule.Id && b.IsActive)
                    .OrderBy(b => b.FromAmount)
                    .ToListAsync(cancellationToken);

                irBrackets = brackets.Select(b => new IR_TaxBracketDto
                {
                    Id = b.Id,
                    ScheduleId = b.ScheduleId,
                    FromAmount = b.FromAmount,
                    ToAmount = b.ToAmount,
                    FixedTax = b.FixedTax,
                    MarginalRate = b.MarginalRate
                }).ToList();
            }
        }

        var inssHistory = await _context.INSSConfigs
                .OrderByDescending(c => c.EffectiveFrom)
                .Select(c => new INSSConfigDto
                {
                    Year = c.EffectiveFrom.Year,
                    RateWorker = c.RateWorker,
                    RateEmployerSmall = c.RateEmployerSmall,
                    RateEmployerLarge = c.RateEmployerLarge,
                    MaxSalaryCap = c.MaxSalaryCap,
                    LegalReference = c.LegalReference,
                    EffectiveFrom = c.EffectiveFrom,
                    EffectiveTo = c.EffectiveTo
                })
                .ToListAsync(cancellationToken);

        // =================================================================
        // 6. MINIMUM WAGES
        // =================================================================
        var minWages = new List<MinimumWageDto>();
        if (targetMitrabYear != 0)
        {
            var mwSchedule = await _context.MinimumWageSchedules
                .Where(s => s.EffectiveFrom.Year == targetMitrabYear && s.IsActive)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefaultAsync(cancellationToken);

            if (mwSchedule != null)
            {
                var wages = await _context.MinimumWages
                    .Where(w => w.ScheduleId == mwSchedule.Id && w.IsActive)
                    .OrderBy(w => w.Sector)
                    .ToListAsync(cancellationToken);

                minWages = wages.Select(w => new MinimumWageDto
                {
                    Id = w.Id,
                    ScheduleId = w.ScheduleId,
                    Sector = w.Sector,
                    MonthlyAmountNIO = w.MonthlyAmountNIO,
                    MonthlyAmountUSD = w.MonthlyAmountUSD
                }).ToList();
            }
        }

        // =================================================================
        // 7. ÚLTIMO TIPO DE CAMBIO
        // =================================================================
        var latestRate = await _context.CurrencyExchanges
            .OrderByDescending(c => c.Date)
            .Select(c => new CurrencyExchangeDto
            {
                Date = c.Date,
                BCNRate = c.BCNRate,
                Source = c.Source ?? ""
            }).FirstOrDefaultAsync(cancellationToken) ?? new CurrencyExchangeDto();

        // =================================================================
        // 8. RETORNAR DTO COMPLETO
        // =================================================================
        return new GlobalConfigDto
        {
            INSSConfig = inssConfig,
            INATECConfig = inatecConfig,
            IRTaxBrackets = irBrackets,
            MinimumWages = minWages,
            LatestExchangeRate = latestRate,
            AvailableINSSYears = availableINSSYears,
            AvailableIRYears = availableIRYears,
            AvailableMitrabYears = availableMitrabYears,
            AvailableINATECYears = availableINATECYears,
            INSSHistory = inssHistory
        };
    }
}