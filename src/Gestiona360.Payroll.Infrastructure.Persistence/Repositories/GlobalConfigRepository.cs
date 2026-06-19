// src/Gestiona360.Payroll.Infrastructure.Persistence/Repositories/GlobalConfigRepository.cs

using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class GlobalConfigRepository : IGlobalConfigRepository
{
    private readonly ApplicationDbContext _context;

    public GlobalConfigRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<GlobalConfigDto> GetGlobalConfigAsync(
        int? yearINSS,
        int? yearIR,
        int? yearMitrab,
        int? yearINATEC,
        CancellationToken ct = default)
    {
        var result = new GlobalConfigDto();

        // 1. Años disponibles
        result.AvailableINSSYears = await _context.INSSConfigs
            .AsNoTracking()
            .Select(c => c.EffectiveFrom.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(ct);

        result.AvailableIRYears = await _context.IrTaxSchedules
            .AsNoTracking()
            .Select(s => s.EffectiveFrom.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(ct);

        result.AvailableMitrabYears = await _context.MinimumWageSchedules
            .AsNoTracking()
            .Select(s => s.EffectiveFrom.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(ct);

        result.AvailableINATECYears = await _context.INATECConfigs
            .AsNoTracking()
            .Select(c => c.EffectiveFrom.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(ct);

        // 2. Determinar años objetivo
        int targetINSSYear = yearINSS ?? result.AvailableINSSYears.FirstOrDefault();
        int targetIRYear = yearIR ?? result.AvailableIRYears.FirstOrDefault();
        int targetMitrabYear = yearMitrab ?? result.AvailableMitrabYears.FirstOrDefault();
        int targetINATECYear = yearINATEC ?? result.AvailableINATECYears.FirstOrDefault();

        // 3. INSS Config
        if (targetINSSYear != 0)
        {
            var inssEntity = await _context.INSSConfigs
                .AsNoTracking()
                .Where(c => c.EffectiveFrom.Year == targetINSSYear && c.IsActive)
                .OrderByDescending(c => c.EffectiveFrom)
                .FirstOrDefaultAsync(ct);

            if (inssEntity != null)
            {
                result.INSSConfig = new INSSConfigDto
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

        // 4. INATEC Config
        if (targetINATECYear != 0)
        {
            var inatecEntity = await _context.INATECConfigs
                .AsNoTracking()
                .Where(c => c.EffectiveFrom.Year == targetINATECYear && c.IsActive)
                .OrderByDescending(c => c.EffectiveFrom)
                .FirstOrDefaultAsync(ct);

            if (inatecEntity != null)
            {
                result.INATECConfig = new INATECConfigDto
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

        // 5. IR Tax Brackets
        if (targetIRYear != 0)
        {
            var irSchedule = await _context.IrTaxSchedules
                .AsNoTracking()
                .Where(s => s.EffectiveFrom.Year == targetIRYear && s.IsActive)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefaultAsync(ct);

            if (irSchedule != null)
            {
                result.IRTaxBrackets = await _context.IR_TaxBrackets
                    .AsNoTracking()
                    .Where(b => b.ScheduleId == irSchedule.Id && b.IsActive)
                    .OrderBy(b => b.FromAmount)
                    .Select(b => new IR_TaxBracketDto
                    {
                        Id = b.Id,
                        ScheduleId = b.ScheduleId,
                        FromAmount = b.FromAmount,
                        ToAmount = b.ToAmount,
                        FixedTax = b.FixedTax,
                        MarginalRate = b.MarginalRate
                    })
                    .ToListAsync(ct);
            }
        }

        // 6. INSS History
        result.INSSHistory = await _context.INSSConfigs
            .AsNoTracking()
            .OrderByDescending(c => c.EffectiveFrom)
            .Select(c => new INSSConfigDto
            {
                Year = c.EffectiveFrom.Year,
                RateWorker = c.RateWorker,
                RateEmployerSmall = c.RateEmployerSmall,
                RateEmployerLarge = c.RateEmployerLarge,
                MaxSalaryCap = c.MaxSalaryCap,
                LegalReference = c.LegalReference ?? "",
                EffectiveFrom = c.EffectiveFrom,
                EffectiveTo = c.EffectiveTo
            })
            .ToListAsync(ct);

        // 7. Minimum Wages
        if (targetMitrabYear != 0)
        {
            var mwSchedule = await _context.MinimumWageSchedules
                .AsNoTracking()
                .Where(s => s.EffectiveFrom.Year == targetMitrabYear && s.IsActive)
                .OrderByDescending(s => s.EffectiveFrom)
                .FirstOrDefaultAsync(ct);

            if (mwSchedule != null)
            {
                result.MinimumWages = await _context.MinimumWages
                    .AsNoTracking()
                    .Where(w => w.ScheduleId == mwSchedule.Id && w.IsActive)
                    .OrderBy(w => w.Sector)
                    .Select(w => new MinimumWageDto
                    {
                        Id = w.Id,
                        ScheduleId = w.ScheduleId,
                        Sector = w.Sector,
                        MonthlyAmountNIO = w.MonthlyAmountNIO,
                        MonthlyAmountUSD = w.MonthlyAmountUSD
                    })
                    .ToListAsync(ct);
            }
        }

        // 8. Latest Exchange Rate
        result.LatestExchangeRate = await _context.CurrencyExchanges
            .AsNoTracking()
            .OrderByDescending(c => c.Date)
            .Select(c => new CurrencyExchangeDto
            {
                Date = c.Date,
                BCNRate = c.BCNRate,
                Source = c.Source ?? ""
            })
            .FirstOrDefaultAsync(ct) ?? new CurrencyExchangeDto();

        return result;
    }
}