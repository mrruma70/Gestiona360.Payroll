// src/Gestiona360.Payroll.Infrastructure.Persistence/Seeding/DbInitializer.cs
using DocumentFormat.OpenXml.Bibliography;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Seeding
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // =================================================================
            // 1. CATÁLOGOS BÁSICOS
            // =================================================================

            // Frecuencias de pago
            if (!await context.PayrollFrequencies.AnyAsync())
            {
                var frequencies = new List<PayrollFrequency>
                {
                    new() { Name = "Mensual", Code = "MONTHLY", DaysPerPeriod = 30, PeriodsPerYear = 12, Description = "Pago una vez al mes", IsActive = true },
                    new() { Name = "Quincenal", Code = "SEMIMONTHLY", DaysPerPeriod = 15, PeriodsPerYear = 24, Description = "Pago dos veces al mes", IsActive = true },
                    new() { Name = "Catorcenal", Code = "BIWEEKLY", DaysPerPeriod = 14, PeriodsPerYear = 26, Description = "Pago cada 14 días", IsActive = true },
                    new() { Name = "Semanal", Code = "WEEKLY", DaysPerPeriod = 7, PeriodsPerYear = 52, Description = "Pago cada semana", IsActive = true }
                };
                await context.PayrollFrequencies.AddRangeAsync(frequencies);
                await context.SaveChangesAsync();
            }

            // Tipos de contrato
            if (!await context.ContractTypes.AnyAsync())
            {
                var contractTypes = new List<ContractType>
                {
                    new() { Name = "Indefinido", DurationType = "Indefinido", WorkdayType = "Completa", SalaryCalcType = "MensualFijo",
                            ApplyINSS = true, ApplyIR = true, ApplyINATEC = true, ApplyVacationsDays = 30, ApplyThirteenthMonth = true,
                            ApplyIndemnity = true, NoticeDays = 15, OvertimeRules = "{\"maxDaily\":3,\"maxWeekly\":9}",
                            DefaultPayComponents = "[\"SAL_BASE\",\"INSS\",\"IR\"]",
                            AllowsBenefitsInKind = false, IsTrustPosition = false, ProbationDays = 0,
                            RequiresWorkPermit = false, HasProbationPeriod = false, IsActive = true },
                    new() { Name = "Por Destajo", DurationType = "Indefinido", WorkdayType = "Variable", SalaryCalcType = "PorDestajo",
                            ApplyINSS = true, ApplyIR = true, ApplyINATEC = true, ApplyVacationsDays = 30, ApplyThirteenthMonth = true,
                            ApplyIndemnity = true, NoticeDays = 15, OvertimeRules = "{\"maxDaily\":3,\"maxWeekly\":9}",
                            DefaultPayComponents = "[\"SAL_BASE\",\"INSS\",\"IR\"]",
                            AllowsBenefitsInKind = false, IsTrustPosition = false, ProbationDays = 30,
                            RequiresWorkPermit = false, HasProbationPeriod = true, IsActive = true },
                    new() { Name = "Plazo fijo", DurationType = "PlazoFijo", WorkdayType = "Completa", SalaryCalcType = "MensualFijo",
                            ApplyINSS = true, ApplyIR = true, ApplyINATEC = true, ApplyVacationsDays = 30, ApplyThirteenthMonth = true,
                            ApplyIndemnity = true, NoticeDays = 0, OvertimeRules = "{\"maxDaily\":3,\"maxWeekly\":9}",
                            DefaultPayComponents = "[\"SAL_BASE\",\"INSS\",\"IR\"]",
                            AllowsBenefitsInKind = false, IsTrustPosition = false, ProbationDays = 0,
                            RequiresWorkPermit = false, HasProbationPeriod = false, IsActive = true },
                    new() { Name = "Período de prueba", DurationType = "Temporal", WorkdayType = "Completa", SalaryCalcType = "MensualFijo",
                            ApplyINSS = true, ApplyIR = true, ApplyINATEC = true, ApplyVacationsDays = 30, ApplyThirteenthMonth = true,
                            ApplyIndemnity = true, NoticeDays = 0, OvertimeRules = "{\"maxDaily\":3,\"maxWeekly\":9}",
                            DefaultPayComponents = "[\"SAL_BASE\",\"INSS\",\"IR\"]",
                            AllowsBenefitsInKind = false, IsTrustPosition = false, ProbationDays = 30,
                            RequiresWorkPermit = false, HasProbationPeriod = true, IsActive = true },
                    new() { Name = "Servicios profesionales", DurationType = "Temporal", WorkdayType = "PorHoras", SalaryCalcType = "PorHora",
                            IncludesServices = true, ApplyINSS = false, ApplyIR = false, ApplyINATEC = false, ApplyVacationsDays = 0,
                            ApplyThirteenthMonth = false, ApplyIndemnity = false, NoticeDays = 0, OvertimeRules = "{}",
                            DefaultPayComponents = "[\"HONORARIOS\"]",
                            AllowsBenefitsInKind = false, IsTrustPosition = false, ProbationDays = 0,
                            RequiresWorkPermit = false, HasProbationPeriod = false, IsActive = true }
                };
                await context.ContractTypes.AddRangeAsync(contractTypes);
                await context.SaveChangesAsync();
            }

            // Riesgos ocupacionales
            if (!await context.OccupationalRisks.AnyAsync())
            {
                var risks = new List<OccupationalRisk>
                {
                    new() { Code = "R01", Name = "Bajo", Description = "Oficinas administrativas", INSSRiskRate = 0.0m, LegalReference = "Ley 618", IsActive = true },
                    new() { Code = "R02", Name = "Medio", Description = "Operarios con maquinaria", INSSRiskRate = 0.5m, LegalReference = "Ley 618", IsActive = true },
                    new() { Code = "R03", Name = "Alto", Description = "Construcción, electricidad, soldadura", INSSRiskRate = 1.0m, LegalReference = "Ley 618", IsActive = true }
                };
                await context.OccupationalRisks.AddRangeAsync(risks);
                await context.SaveChangesAsync();
            }

            // =================================================================
            // CONFIGURACIONES FISCALES
            // =================================================================

            if (!await context.INSSConfigs.AnyAsync())
            {
                var inssConfig = new INSSConfig
                {
                    EffectiveFrom = new DateOnly(2026, 1, 1),
                    EffectiveTo = null,
                    LegalReference = "Convenio INSS 2026",
                    RateWorker = 7.0m,
                    RateEmployerSmall = 21.5m,
                    RateEmployerLarge = 22.5m,
                    MaxSalaryCap = 50000.00m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await context.INSSConfigs.AddAsync(inssConfig);
                await context.SaveChangesAsync();
            }

            if (!await context.INATECConfigs.AnyAsync())
            {
                var inatecConfig = new INATECConfig
                {
                    EffectiveFrom = new DateOnly(2026, 1, 1),
                    EffectiveTo = null,
                    LegalReference = "Ley 1063",
                    Rate = 2.00m,
                    Exceptions = "MINED, MINSA",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await context.INATECConfigs.AddAsync(inatecConfig);
                await context.SaveChangesAsync();
            }

            IrTaxSchedule? irSchedule = null;
            if (!await context.IrTaxSchedules.AnyAsync())
            {
                irSchedule = new IrTaxSchedule
                {
                    EffectiveFrom = new DateOnly(2026, 1, 1),
                    EffectiveTo = null,
                    LegalReference = "Ley 822 + Reforma 891",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await context.IrTaxSchedules.AddAsync(irSchedule);
                await context.SaveChangesAsync();

                var irBrackets = new List<IR_TaxBracket>
                {
                    new() { ScheduleId = irSchedule.Id, FromAmount = 0, ToAmount = 100000, FixedTax = 0, MarginalRate = 0, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = irSchedule.Id, FromAmount = 100000.01m, ToAmount = 200000, FixedTax = 0, MarginalRate = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = irSchedule.Id, FromAmount = 200000.01m, ToAmount = 350000, FixedTax = 15000, MarginalRate = 20, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = irSchedule.Id, FromAmount = 350000.01m, ToAmount = 500000, FixedTax = 45000, MarginalRate = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = irSchedule.Id, FromAmount = 500000.01m, ToAmount = null, FixedTax = 82500, MarginalRate = 30, IsActive = true, CreatedAt = DateTime.UtcNow }
                };
                await context.IR_TaxBrackets.AddRangeAsync(irBrackets);
                await context.SaveChangesAsync();
            }
            else
            {
                irSchedule = await context.IrTaxSchedules
                    .Where(s => s.EffectiveTo == null && s.IsActive)
                    .OrderByDescending(s => s.EffectiveFrom)
                    .FirstOrDefaultAsync();
            }

            MinimumWageSchedule? mwSchedule = null;
            if (!await context.MinimumWageSchedules.AnyAsync())
            {
                mwSchedule = new MinimumWageSchedule
                {
                    EffectiveFrom = new DateOnly(2026, 1, 1),
                    EffectiveTo = null,
                    LegalReference = "Acuerdo MITRAB 2026",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await context.MinimumWageSchedules.AddAsync(mwSchedule);
                await context.SaveChangesAsync();

                var minWages = new List<MinimumWage>
                {
                    new() { ScheduleId = mwSchedule.Id, Sector = "Agropecuario", MonthlyAmountNIO = 5450.75m, MonthlyAmountUSD = 147.91m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = mwSchedule.Id, Sector = "Pesca", MonthlyAmountNIO = 8200.00m, MonthlyAmountUSD = 222.52m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = mwSchedule.Id, Sector = "Minas y Canteras", MonthlyAmountNIO = 10500.30m, MonthlyAmountUSD = 284.94m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = mwSchedule.Id, Sector = "Industria Manufacturera", MonthlyAmountNIO = 7600.00m, MonthlyAmountUSD = 206.24m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = mwSchedule.Id, Sector = "Industrias Sujetas a Régimen de Zona Franca", MonthlyAmountNIO = 8750.25m, MonthlyAmountUSD = 237.45m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = mwSchedule.Id, Sector = "Micro y Pequeña Industria Artesanal y Turística", MonthlyAmountNIO = 6100.00m, MonthlyAmountUSD = 165.53m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = mwSchedule.Id, Sector = "Electricidad Gas y Agua Comercio Restaurantes", MonthlyAmountNIO = 9950.00m, MonthlyAmountUSD = 270.01m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = mwSchedule.Id, Sector = "Construcción Establecimientos Financieros y Seguros", MonthlyAmountNIO = 12500.80m, MonthlyAmountUSD = 339.23m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = mwSchedule.Id, Sector = "Servicios Comunales Sociales y Personales", MonthlyAmountNIO = 7900.00m, MonthlyAmountUSD = 214.38m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { ScheduleId = mwSchedule.Id, Sector = "Gobierno Central y Municipal", MonthlyAmountNIO = 7400.00m, MonthlyAmountUSD = 200.81m, IsActive = true, CreatedAt = DateTime.UtcNow }
                };
                await context.MinimumWages.AddRangeAsync(minWages);
                await context.SaveChangesAsync();
            }
            else
            {
                mwSchedule = await context.MinimumWageSchedules
                    .Where(s => s.EffectiveTo == null && s.IsActive)
                    .OrderByDescending(s => s.EffectiveFrom)
                    .FirstOrDefaultAsync();
            }

            if (!await context.HolidayCalendars.AnyAsync())
            {
                var holidays = new List<HolidayCalendar>
                {
                    new() { Year = 2026, Date = new DateTime(2026, 1, 1), Description = "Año Nuevo", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 1, 18), Description = "Feriado Nuevo (Ley 1272)", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 2, 2), Description = "Feriado Nuevo (Ley 1272)", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 2, 21), Description = "Feriado Nuevo (Ley 1272)", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 4, 2), Description = "Jueves Santo", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 4, 3), Description = "Viernes Santo", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 5, 1), Description = "Día del Trabajo", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 7, 19), Description = "Revolución", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 9, 14), Description = "Batalla San Jacinto", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 9, 15), Description = "Independencia", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 11, 8), Description = "Feriado Nuevo (Ley 1272)", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 12, 8), Description = "Inmaculada Concepción", IsPaidDouble = true, IsActive = true },
                    new() { Year = 2026, Date = new DateTime(2026, 12, 25), Description = "Navidad", IsPaidDouble = true, IsActive = true }
                };
                await context.HolidayCalendars.AddRangeAsync(holidays);
                await context.SaveChangesAsync();
            }

            if (!await context.PayrollConcepts.AnyAsync())
            {
                var concepts = new List<PayrollConcept>
                {
                    new() { Code = "SAL_BASE", Name = "Salario Base", Type = "Perception", Category = "Contractual", Priority = 1,
                            ApplyToINSS = true, ApplyToIR = true, ApplyToINATEC = true, CalculationMethod = "FixedAmount", CalculationValue = 0m,
                            IsRecurrentByDefault = true, ShowInPaySlipAsDetail = true, IsActive = true,
                            LegalReference = "Art. 49 Código del Trabajo" },
                    new() { Code = "HORAS_EXTRA", Name = "Horas Extras", Type = "Perception", Category = "Contractual", Priority = 1,
                            ApplyToINSS = true, ApplyToIR = true, ApplyToINATEC = true, CalculationMethod = "PerHour", CalculationValue = 0m,
                            IsRecurrentByDefault = false, ShowInPaySlipAsDetail = true, IsActive = true,
                            LegalReference = "Art. 62 Código del Trabajo" },
                    new() { Code = "INSS", Name = "Aporte INSS (7%)", Type = "Deduction", Category = "Legal", Priority = 1,
                            ApplyToINSS = false, ApplyToIR = false, ApplyToINATEC = false, CalculationMethod = "PercentageOfGross", CalculationValue = 7m,
                            IsRecurrentByDefault = true, ShowInPaySlipAsDetail = true, IsActive = true,
                            LegalReference = "Ley 539, Decreto 974" },
                    new() { Code = "IR", Name = "Impuesto sobre la Renta", Type = "Deduction", Category = "Legal", Priority = 2,
                            ApplyToINSS = false, ApplyToIR = false, ApplyToINATEC = false, CalculationMethod = "Formula", CalculationValue = 0m,
                            IsRecurrentByDefault = true, ShowInPaySlipAsDetail = true, IsActive = true,
                            LegalReference = "Ley 822, reforma 891" },
                    new() { Code = "PRESTAMO", Name = "Préstamo Empresarial", Type = "Deduction", Category = "Contractual", Priority = 3,
                            ApplyToINSS = false, ApplyToIR = false, ApplyToINATEC = false, CalculationMethod = "FixedAmount", CalculationValue = 0m,
                            IsRecurrentByDefault = false, CanBeReprogrammed = true, ShowInPaySlipAsDetail = true, IsActive = true,
                            LegalReference = "Acuerdo empresa - empleado" }
                };
                await context.PayrollConcepts.AddRangeAsync(concepts);
                await context.SaveChangesAsync();
            }

            if (!await context.Banks.AnyAsync())
            {
                var now = DateTime.UtcNow;
                var banks = new List<Bank>
                {
                    new() { Name = "Banco de América Central (BAC)", Code = "BAC", AchCode = "BAC", AccountNumberPrefix = "BAC", IsActive = true, CreatedAt = now },
                    new() { Name = "Banco de la Producción (Banpro)", Code = "BANPRO", AchCode = "BANPRO", AccountNumberPrefix = "BANPRO", IsActive = true, CreatedAt = now },
                    new() { Name = "Banco Lafise", Code = "LAFISE", AchCode = "LAFISE", AccountNumberPrefix = "LAFISE", IsActive = true, CreatedAt = now },
                    new() { Name = "Banco Ficohsa Nicaragua", Code = "FICOHSA", AchCode = "FICOHSA", AccountNumberPrefix = "FICOHSA", IsActive = true, CreatedAt = now },
                    new() { Name = "Banco ProCredit", Code = "PROCREDIT", AccountNumberPrefix = "PROCREDIT", IsActive = true, CreatedAt = now },
                    new() { Name = "Banco Avanz", Code = "AVANZ", AccountNumberPrefix = "AVANZ", IsActive = true, CreatedAt = now },
                    new() { Name = "Banco Atlántida", Code = "ATLANTIDA", AccountNumberPrefix = "ATLANTIDA", IsActive = true, CreatedAt = now }
                };
                await context.Banks.AddRangeAsync(banks);
                await context.SaveChangesAsync();
            }

            // =================================================================
            // 2. ENTIDADES CON DEPENDENCIAS
            // =================================================================

            var defaultFrequency = await context.PayrollFrequencies.FirstOrDefaultAsync(f => f.Code == "MONTHLY");
            var lowRisk = await context.OccupationalRisks.FirstOrDefaultAsync(r => r.Code == "R01");
            var servicioMinimum = await context.MinimumWages.FirstOrDefaultAsync(m => m.Sector == "Servicios Comunales Sociales y Personales");

            if (!await context.Companies.AnyAsync())
            {
                var company = new Company
                {
                    LegalName = "Distribuidora Comercial de Nicaragua, S.A.",
                    CommercialName = "Distribuidora Comercial",
                    TaxId = "J0310000012345",
                    INSSEmployerCode = "1234567-8",
                    EconomicActivityCode = "4659",
                    TotalActiveEmployees = 0,
                    Phone = "+505 2255 1234",
                    Email = "contacto@distribuidora.com.ni",
                    Address = "De los semáforos de Enel Central, 200 varas al sur, Managua",
                    City = "Managua",
                    Department = "Managua",
                    LegalRepresentative = "Juan Pérez García",
                    LegalRepresentativeId = "001-010285-1234K",
                    DefaultCurrency = "NIO",
                    DefaultPayrollFrequencyId = defaultFrequency!.Id,
                    DefaultIsZoneFranca = false,
                    IsActive = true,
                    LogoUrl = ""
                };
                await context.Companies.AddAsync(company);
                await context.SaveChangesAsync();
            }

            var existingCompany = await context.Companies.FirstOrDefaultAsync();

            if (!await context.Branches.AnyAsync() && existingCompany != null)
            {
                var branches = new List<Branch>
                {
                    new() { Code = "MGA-01", Name = "Sede Central Managua", Address = existingCompany.Address, City = "Managua", Phone = existingCompany.Phone, CompanyId = existingCompany.Id, IsActive = true, IsZoneFranca = false },
                    new() { Code = "EST-02", Name = "Sucursal Norte Estelí", Address = "Contiguo a la salida sur de Estelí, Km 148", City = "Estelí", Phone = "+505 2713 5678", CompanyId = existingCompany.Id, IsActive = true, IsZoneFranca = false }
                };
                await context.Branches.AddRangeAsync(branches);
                await context.SaveChangesAsync();
            }

            var branchMGA = await context.Branches.FirstOrDefaultAsync(b => b.Code == "MGA-01");
            var branchEST = await context.Branches.FirstOrDefaultAsync(b => b.Code == "EST-02");

            if (!await context.CostCenters.AnyAsync() && branchMGA != null && branchEST != null)
            {
                var costCenters = new List<CostCenter>
                {
                    new() { Code = "100-ADM", Name = "Administración General", CostType = "Administrativo", BranchId = branchMGA.Id, IsActive = true, ParentCostCenterId = null },
                    new() { Code = "200-VTA", Name = "Ventas y Distribución", CostType = "Operativo", BranchId = branchMGA.Id, IsActive = true, ParentCostCenterId = null },
                    new() { Code = "250-VTA-EST", Name = "Ventas Estelí", CostType = "Operativo", BranchId = branchEST.Id, IsActive = true, ParentCostCenterId = null }
                };
                costCenters[1].ParentCostCenterId = costCenters[0].Id;
                costCenters[2].ParentCostCenterId = costCenters[1].Id;
                await context.CostCenters.AddRangeAsync(costCenters);
                await context.SaveChangesAsync();
            }

            if (!await context.HealthProviders.AnyAsync() && existingCompany != null)
            {
                var healthProvider = new HealthProvider
                {
                    Name = "Hospital Metropolitano Vivian Pellas",
                    Type = "Hospital",
                    ContactPhone = "+505 2250 0050",
                    Address = "Carretera a Masaya, Km 9.5, Managua",
                    CompanyId = existingCompany.Id,
                    IsActive = true
                };
                await context.HealthProviders.AddAsync(healthProvider);
                await context.SaveChangesAsync();
            }

            var healthProviderEntity = await context.HealthProviders.FirstOrDefaultAsync();

            if (!await context.PayrollGroups.AnyAsync() && existingCompany != null && defaultFrequency != null)
            {
                var payrollGroup = new PayrollGroup
                {
                    Name = "Planilla Mensual Oficinas",
                    FrequencyId = defaultFrequency.Id,
                    CostCenterCode = "100-ADM",
                    ClosingDayRule = 28,
                    FirstPeriodStartDate = new DateTime(2026, 1, 1),
                    IsActive = true
                };
                await context.PayrollGroups.AddAsync(payrollGroup);
                await context.SaveChangesAsync();
            }

            var payrollGroupEntity = await context.PayrollGroups.FirstOrDefaultAsync();

            // =================================================================
            // 3. UBICACIONES (DEPARTAMENTOS Y MUNICIPIOS) - CORREGIDO
            // =================================================================

            await LocationSeeder.SeedAsync(context);

            // Obtener referencias a departamentos y municipios
            var deptManagua = await context.Departments.FirstOrDefaultAsync(d => d.Code == "MGA");
            var munManagua = await context.Municipalities.FirstOrDefaultAsync(m => m.Name == "Managua" && m.DepartmentId == deptManagua!.Id);
            var munTipitapa = await context.Municipalities.FirstOrDefaultAsync(m => m.Name == "Tipitapa" && m.DepartmentId == deptManagua!.Id);

            // =================================================================
            // 4. PUESTOS Y GRADOS
            // =================================================================

            var jobPositionIds = new Dictionary<string, Guid>();

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "ADM-001"))
            {
                var admId = Guid.Parse("02BF6A4C-A54F-4CAA-B99D-FD1231B36833");
                var admPosition = new JobPosition
                {
                    Id = admId,
                    Code = "ADM-001",
                    Name = "Asistente Administrativo",
                    Category = "Administrativo",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Apoyo en tareas administrativas, manejo de correspondencia y archivo.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(admPosition);
                jobPositionIds["ADM-001"] = admId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "VEN-001"))
            {
                var venId = Guid.Parse("EB742949-57A3-4735-A835-AE645FC803EF");
                var venPosition = new JobPosition
                {
                    Id = venId,
                    Code = "VEN-001",
                    Name = "Vendedor",
                    Category = "Ventas",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Atención al cliente, promoción y cierre de ventas.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(venPosition);
                jobPositionIds["VEN-001"] = venId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "CAJ-001"))
            {
                var cajId = Guid.Parse("094B8ADE-81A0-48EE-8F16-9F973432E415");
                var cajPosition = new JobPosition
                {
                    Id = cajId,
                    Code = "CAJ-001",
                    Name = "Cajero",
                    Category = "Operativo",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Manejo de caja, registro de transacciones y atención al público.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(cajPosition);
                jobPositionIds["CAJ-001"] = cajId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "SEC-001"))
            {
                var secId = Guid.Parse("B0C3E671-C71A-456D-887C-BC55E2444E59");
                var secPosition = new JobPosition
                {
                    Id = secId,
                    Code = "SEC-001",
                    Name = "Secretaria",
                    Category = "Administrativo",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Recepción, gestión de agenda y soporte administrativo.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(secPosition);
                jobPositionIds["SEC-001"] = secId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "CON-001"))
            {
                var conId = Guid.Parse("9F25FDE0-58BC-431E-B8F7-35295D045942");
                var conPosition = new JobPosition
                {
                    Id = conId,
                    Code = "CON-001",
                    Name = "Conductor",
                    Category = "Operativo",
                    OccupationalRiskId = 2,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Transporte de personal o mercancías, mantenimiento básico del vehículo.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(conPosition);
                jobPositionIds["CON-001"] = conId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "ALM-001"))
            {
                var almId = Guid.Parse("2430CB9B-CB7B-4F0B-94C6-A55B48948842");
                var almPosition = new JobPosition
                {
                    Id = almId,
                    Code = "ALM-001",
                    Name = "Almacenista",
                    Category = "Operativo",
                    OccupationalRiskId = 2,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Control de inventario, recepción y despacho de mercancías.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(almPosition);
                jobPositionIds["ALM-001"] = almId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "SEG-001"))
            {
                var segId = Guid.Parse("2EF3919F-8D15-4F9F-9001-6FC8524F805B");
                var segPosition = new JobPosition
                {
                    Id = segId,
                    Code = "SEG-001",
                    Name = "Guarda de Seguridad",
                    Category = "Operativo",
                    OccupationalRiskId = 2,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Vigilancia de instalaciones, control de accesos y rondas de seguridad.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(segPosition);
                jobPositionIds["SEG-001"] = segId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "LIM-001"))
            {
                var limId = Guid.Parse("3250E597-45F5-4104-A7CA-A1E8C2303D57");
                var limPosition = new JobPosition
                {
                    Id = limId,
                    Code = "LIM-001",
                    Name = "Limpiador",
                    Category = "Operativo",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Limpieza y desinfección de áreas comunes y oficinas.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(limPosition);
                jobPositionIds["LIM-001"] = limId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "TEC-001"))
            {
                var tecId = Guid.Parse("9204DB9D-1192-4395-872B-589298F3B22B");
                var tecPosition = new JobPosition
                {
                    Id = tecId,
                    Code = "TEC-001",
                    Name = "Técnico en TI",
                    Category = "Tecnología",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Soporte técnico, mantenimiento de equipos y redes.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(tecPosition);
                jobPositionIds["TEC-001"] = tecId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "RRHH-001"))
            {
                var rrhhId = Guid.Parse("4C7ED2E2-CCE6-4E74-825B-34CA44EE9308");
                var rrhhPosition = new JobPosition
                {
                    Id = rrhhId,
                    Code = "RRHH-001",
                    Name = "Asistente de RR.HH.",
                    Category = "Administrativo",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Reclutamiento, gestión de expedientes y nómina.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(rrhhPosition);
                jobPositionIds["RRHH-001"] = rrhhId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "MKT-001"))
            {
                var mktId = Guid.Parse("12ACCDFC-A4B5-445D-A036-B1BB20F7E65A");
                var mktPosition = new JobPosition
                {
                    Id = mktId,
                    Code = "MKT-001",
                    Name = "Coordinador de Marketing",
                    Category = "Administrativo",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Elaboración de estrategias de marketing y gestión de redes sociales.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(mktPosition);
                jobPositionIds["MKT-001"] = mktId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "PROD-001"))
            {
                var prodId = Guid.Parse("E96E26F9-312F-45CE-A62D-8D4FC4DBBC77");
                var prodPosition = new JobPosition
                {
                    Id = prodId,
                    Code = "PROD-001",
                    Name = "Supervisor de Producción",
                    Category = "Operativo",
                    OccupationalRiskId = 2,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Supervisión de línea de producción, control de calidad y seguridad.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(prodPosition);
                jobPositionIds["PROD-001"] = prodId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "CAL-001"))
            {
                var calId = Guid.Parse("87AC47F7-8404-4107-9BB8-11AECD11ADB9");
                var calPosition = new JobPosition
                {
                    Id = calId,
                    Code = "CAL-001",
                    Name = "Inspector de Calidad",
                    Category = "Operativo",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Inspección de productos, elaboración de reportes de calidad.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(calPosition);
                jobPositionIds["CAL-001"] = calId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "COM-001"))
            {
                var comId = Guid.Parse("846CA715-3E6F-4A23-A2C8-093AAC992E80");
                var comPosition = new JobPosition
                {
                    Id = comId,
                    Code = "COM-001",
                    Name = "Comprador",
                    Category = "Administrativo",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Negociación con proveedores, gestión de órdenes de compra.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(comPosition);
                jobPositionIds["COM-001"] = comId;
            }

            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "REC-001"))
            {
                var recId = Guid.Parse("4A28DC5C-84DB-4E64-B20D-9E7D486EF9AB");
                var recPosition = new JobPosition
                {
                    Id = recId,
                    Code = "REC-001",
                    Name = "Recepcionista",
                    Category = "Administrativo",
                    OccupationalRiskId = lowRisk!.Id,
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Atención a visitantes, manejo de llamadas y correspondencia.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(recPosition);
                jobPositionIds["REC-001"] = recId;
            }

            await context.SaveChangesAsync();

            // Crear grados (JobGrades) - código existente sin cambios...
            // [Mantén aquí todo el código de grados que ya tenías]

            // =================================================================
            // 5. EMPLEADO DEMO - ACTUALIZADO CON UBICACIÓN
            // =================================================================

            var existingDemoEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Code == "EMP-001");
            Employee? employee = null;

            if (existingDemoEmployee == null &&
                existingCompany != null &&
                branchMGA != null &&
                healthProviderEntity != null &&
                lowRisk != null)
            {
                var contractIndefinido = await context.ContractTypes.FirstOrDefaultAsync(c => c.Name == "Indefinido");
                var bacBank = await context.Banks.FirstOrDefaultAsync(b => b.Code == "BAC");
                var inspectorGrade = await context.JobGrades.FirstOrDefaultAsync(jg => jg.Code == "CAL-JR");
                var payrollGroupEntity2 = await context.PayrollGroups.FirstOrDefaultAsync();

                if (contractIndefinido != null)
                {
                    employee = new Employee
                    {
                        Code = "EMP-001",
                        Identification = "001-010285-1234K",
                        FirstName = "Juan",
                        SecondName = "Carlos",
                        LastName = "Pérez",
                        SecondLastName = "López",
                        Email = "juan.perez@distribuidora.com.ni",
                        Phone = "+505 2255 1234",
                        MobilePhone = "+505 8888 1234",
                        HireDate = new DateTime(2025, 1, 15),
                        FirstHireDate = new DateTime(2025, 1, 15),
                        BirthDate = new DateTime(1985, 5, 15),
                        Gender = "M",
                        MaritalStatus = "C",

                        // ✅ NUEVOS: Domicilio
                        Address = "Residencial Las Colinas, Casa #45",
                        DepartmentId = deptManagua?.Id,
                        MunicipalityId = munManagua?.Id,

                        // ✅ NUEVOS: Contacto de emergencia
                        EmergencyContactName = "María López de Pérez",
                        EmergencyContactPhone = "+505 8888 5678",
                        EmergencyContactRelationship = "Esposa",

                        CompanyId = existingCompany.Id,
                        BranchId = branchMGA.Id,
                        ContractTypeId = contractIndefinido.Id,
                        JobGradeId = inspectorGrade?.Id,
                        HealthProviderId = healthProviderEntity.Id,
                        OccupationalRiskId = lowRisk.Id,
                        BaseSalary = 16000.00m,

                        BankId = bacBank?.Id,
                        BankAccountNumber = bacBank != null ? "123456789" : null,
                        BankAccountType = bacBank != null ? "Ahorro" : null,
                        BankBeneficiaryName = "Juan Carlos Pérez López",

                        NOINSS = "001-010285-1234K",
                        NORUC = "RUC123456789",

                        IsActive = true,
                        EmploymentStatus = EmploymentStatus.Active,
                        IsTrustEmployee = false,
                        UsesTimeClock = true,

                        PayrollGroupId = payrollGroupEntity2!.Id,

                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Employees.AddAsync(employee);
                    await context.SaveChangesAsync();
                    Console.WriteLine("✅ Empleado demo creado exitosamente: EMP-001");
                }
            }
            else if (existingDemoEmployee != null)
            {
                employee = existingDemoEmployee;
                Console.WriteLine("ℹ️ Empleado demo ya existe: EMP-001");
            }

            if (employee == null)
            {
                employee = await context.Employees.FirstOrDefaultAsync();
            }

            // Período de nómina
            if (!await context.PayrollPeriods.AnyAsync() && payrollGroupEntity != null)
            {
                var payrollPeriod = new PayrollPeriod
                {
                    PayrollGroupId = payrollGroupEntity.Id,
                    StartDate = new DateTime(2026, 1, 1),
                    EndDate = new DateTime(2026, 1, 31),
                    PeriodNumber = 1,
                    Status = "Open",
                    ClosedHash = ""
                };
                await context.PayrollPeriods.AddAsync(payrollPeriod);
                await context.SaveChangesAsync();
            }

            // Turnos
            if (!await context.Shifts.AnyAsync())
            {
                var turnoDiurno = new Shift
                {
                    Name = "Diurno",
                    ShiftType = "Matutino",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    TotalHours = 8,
                    IsNightShift = false,
                    IsActive = true
                };
                await context.Shifts.AddAsync(turnoDiurno);
                await context.SaveChangesAsync();

                var schedules = new List<Schedule>();
                for (int i = 1; i <= 5; i++)
                {
                    schedules.Add(new Schedule
                    {
                        ShiftId = turnoDiurno.Id,
                        DayOfWeek = (DayOfWeek)i,
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        IsRestDay = false,
                        IsActive = true
                    });
                }
                schedules.Add(new Schedule
                {
                    ShiftId = turnoDiurno.Id,
                    DayOfWeek = DayOfWeek.Saturday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    IsRestDay = false,
                    IsActive = true
                });
                schedules.Add(new Schedule
                {
                    ShiftId = turnoDiurno.Id,
                    DayOfWeek = DayOfWeek.Sunday,
                    StartTime = TimeSpan.Zero,
                    EndTime = TimeSpan.Zero,
                    IsRestDay = true,
                    IsActive = true
                });
                await context.Schedules.AddRangeAsync(schedules);
                await context.SaveChangesAsync();
            }

            // Plan de cuentas
            if (!await context.GLAccounts.AnyAsync())
            {
                var glAccounts = new List<GLAccount>
                {
                    new() { Code = "11001-01", Name = "Sueldos y Salarios (Planta)", AccountType = "Gasto", IsActive = true },
                    new() { Code = "11001-02", Name = "Sueldos y Salarios (Admin)", AccountType = "Gasto", IsActive = true },
                    new() { Code = "11002-01", Name = "Aporte Patronal INSS", AccountType = "Gasto", IsActive = true },
                    new() { Code = "11003-01", Name = "Aporte INATEC", AccountType = "Gasto", IsActive = true },
                    new() { Code = "21001-01", Name = "Retenciones IR por Pagar", AccountType = "Pasivo", IsActive = true },
                    new() { Code = "21002-01", Name = "Aportes INSS por Pagar", AccountType = "Pasivo", IsActive = true },
                    new() { Code = "21003-01", Name = "Préstamos Empleados por Cobrar", AccountType = "Activo", IsActive = true }
                };
                await context.GLAccounts.AddRangeAsync(glAccounts);
                await context.SaveChangesAsync();
            }

            // Mapeo contable
            var salBaseConcept = await context.PayrollConcepts.FirstOrDefaultAsync(c => c.Code == "SAL_BASE");
            var costCenterAdmin = await context.CostCenters.FirstOrDefaultAsync(cc => cc.Code == "100-ADM");
            var cuentaSueldosAdmin = await context.GLAccounts.FirstOrDefaultAsync(g => g.Code == "11001-02");

            if (salBaseConcept != null && costCenterAdmin != null && cuentaSueldosAdmin != null && !await context.PayrollConceptGLMappings.AnyAsync())
            {
                var mapping = new PayrollConceptGLMapping
                {
                    PayrollConceptId = salBaseConcept.Id,
                    CostCenterId = costCenterAdmin.Id,
                    GLAccountId = cuentaSueldosAdmin.Id,
                    IsActive = true
                };
                await context.PayrollConceptGLMappings.AddAsync(mapping);
                await context.SaveChangesAsync();
            }

            // Vacaciones
            if (employee != null && !await context.VacationBalances.AnyAsync(vb => vb.EmployeeId == employee.Id))
            {
                var vacBalance = new VacationBalance
                {
                    EmployeeId = employee.Id,
                    AccruedDays = 0,
                    UsedDays = 0,
                    AvailableDays = 0,
                    ExpiredDays = 0,
                    LastAccrualDate = employee.HireDate,
                    AccrualRateDaysPerSemester = 15
                };
                await context.VacationBalances.AddAsync(vacBalance);
                await context.SaveChangesAsync();
            }

            // Aguinaldo
            if (employee != null && !await context.ThirteenthMonths.AnyAsync(tm => tm.EmployeeId == employee.Id && tm.Year == 2026))
            {
                var aguinaldoProvision = new ThirteenthMonth
                {
                    EmployeeId = employee.Id,
                    Year = 2026,
                    Provision = 0,
                    FinalCalculationAmount = 0,
                    CalculationBaseAmount = 0,
                    PaymentDate = null,
                    PaidStatus = "Pending"
                };
                await context.ThirteenthMonths.AddAsync(aguinaldoProvision);
                await context.SaveChangesAsync();
            }

            // Indemnización
            if (employee != null && !await context.IndemnityProvisions.AnyAsync(ip => ip.EmployeeId == employee.Id))
            {
                var indemnityProvision = new IndemnityProvision
                {
                    EmployeeId = employee.Id,
                    MonthlyProvision = 0,
                    TotalAccrued = 0,
                    Sector = "Privado",
                    LastProvisionDate = employee.HireDate
                };
                await context.IndemnityProvisions.AddAsync(indemnityProvision);
                await context.SaveChangesAsync();
            }

            // Préstamo
            var prestamoConcept = await context.PayrollConcepts.FirstOrDefaultAsync(c => c.Code == "PRESTAMO");
            if (employee != null && prestamoConcept != null && !await context.EmployeeConceptSettings.AnyAsync(ec => ec.EmployeeId == employee.Id && ec.PayrollConceptId == prestamoConcept.Id))
            {
                var loanSetting = new EmployeeConceptSetting
                {
                    EmployeeId = employee.Id,
                    PayrollConceptId = prestamoConcept.Id,
                    StartDate = new DateTime(2026, 1, 1),
                    EndDate = new DateTime(2026, 12, 31),
                    IsActive = true,
                    CustomValue = 1000m,
                    TotalPrincipal = 5000m,
                    RemainingBalance = 5000m,
                    InstallmentAmount = 1000m,
                    InstallmentTotal = 5,
                    InstallmentCurrent = 0,
                    IsRecurrent = true,
                    ApplicationPeriodicity = "Monthly",
                    AutoReprogram = false,
                    LastReprogramReason = ""
                };
                await context.EmployeeConceptSettings.AddAsync(loanSetting);
                await context.SaveChangesAsync();
            }

            Console.WriteLine("✅ Seed completado exitosamente.");
        }
    }

    // =================================================================
    // LOCATION SEEDER - CORREGIDO (SIN IDs EXPLÍCITOS)
    // =================================================================

        public static class LocationSeeder
        {
            public static async Task SeedAsync(ApplicationDbContext context)
            {
            // 1. Declaramos la variable fuera del bloque 'if' obteniendo los datos actuales
            var departments = await context.Departments.ToListAsync();

            // ✅ DEPARTAMENTOS - SIN IDs EXPLÍCITOS
            if (departments.Count == 0)
            {
                departments = new List<Gestiona360.Payroll.Domain.Entities.Department>          
                {
                    new() { Name = "Managua", Code = "MGA" , IsActive = true },
                    new() { Name = "Boaco", Code = "BOA" , IsActive = true },
                    new() { Name = "Carazo", Code = "CAR" , IsActive = true },
                    new() { Name = "Chinandega", Code = "CHI" , IsActive = true },
                    new() { Name = "Chontales", Code = "CHO" , IsActive = true },
                    new() { Name = "Estelí", Code = "EST" , IsActive = true  },
                    new() { Name = "Granada", Code = "GRA" , IsActive = true },
                    new() { Name = "Jinotega", Code = "JIN" , IsActive = true },
                    new() { Name = "León", Code = "LEO" , IsActive = true },
                    new() { Name = "Madriz", Code = "MAD" , IsActive = true },
                    new() { Name = "Masaya", Code = "MAS" , IsActive = true },
                    new() { Name = "Matagalpa", Code = "MAT" , IsActive = true },
                    new() { Name = "Nueva Segovia", Code = "NSE" , IsActive = true },
                    new() { Name = "Río San Juan", Code = "RSJ" , IsActive = true },
                    new() { Name = "Rivas", Code = "RIV" , IsActive = true },
                    new() { Name = "Región Autónoma del Atlántico Norte", Code = "RACCN" , IsActive = true },
                    new() { Name = "Región Autónoma del Atlántico Sur", Code = "RACCS" , IsActive = true }
                };
                    await context.Departments.AddRangeAsync(departments);
                    await context.SaveChangesAsync();
                }

                // ===================== MUNICIPIOS =====================
                if (!await context.Municipalities.AnyAsync())
                {
                    // Obtener departamentos para asociar por Id
                    var deptManagua = await context.Departments.FirstAsync(d => d.Code == "MGA" );
                    var deptBoaco = await context.Departments.FirstAsync(d => d.Code == "BOA");
                    var deptCarazo = await context.Departments.FirstAsync(d => d.Code == "CAR");
                    var deptChinandega = await context.Departments.FirstAsync(d => d.Code == "CHI");
                    var deptChontales = await context.Departments.FirstAsync(d => d.Code == "CHO");
                    var deptEsteli = await context.Departments.FirstAsync(d => d.Code == "EST");
                    var deptGranada = await context.Departments.FirstAsync(d => d.Code == "GRA");
                    var deptJinotega = await context.Departments.FirstAsync(d => d.Code == "JIN");
                    var deptLeon = await context.Departments.FirstAsync(d => d.Code == "LEO");
                    var deptMadriz = await context.Departments.FirstAsync(d => d.Code == "MAD");
                    var deptMasaya = await context.Departments.FirstAsync(d => d.Code == "MAS");
                    var deptMatagalpa = await context.Departments.FirstAsync(d => d.Code == "MAT");
                    var deptNuevaSegovia = await context.Departments.FirstAsync(d => d.Code == "NSE");
                    var deptRioSanJuan = await context.Departments.FirstAsync(d => d.Code == "RSJ");
                    var deptRivas = await context.Departments.FirstAsync(d => d.Code == "RIV");
                    var deptRACCN = await context.Departments.FirstAsync(d => d.Code == "RACCN");
                    var deptRACCS = await context.Departments.FirstAsync(d => d.Code == "RACCS");

                    var municipalities = new List<Municipality>();

                    // ----- Managua (códigos 1 al 9) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Managua", Code = 1, DepartmentId = deptManagua.Id, IsActive = true },
                    new Municipality { Name = "San Rafael Del Sur", Code = 2, DepartmentId = deptManagua.Id, IsActive = true },
                    new Municipality { Name = "Tipitapa", Code = 3, DepartmentId = deptManagua.Id, IsActive = true },
                    new Municipality { Name = "Villa Carlos Fonseca", Code = 4, DepartmentId = deptManagua.Id, IsActive = true },
                    new Municipality { Name = "San Francisco Libre", Code = 5, DepartmentId = deptManagua.Id, IsActive = true },
                    new Municipality { Name = "Mateare", Code = 6, DepartmentId = deptManagua.Id, IsActive = true },
                    new Municipality { Name = "Ticuantepe", Code = 7, DepartmentId = deptManagua.Id, IsActive = true },
                    new Municipality { Name = "Ciudad Sandino", Code = 8, DepartmentId = deptManagua.Id, IsActive = true },
                    new Municipality { Name = "El Crucero", Code = 9, DepartmentId = deptManagua.Id, IsActive = true }
                });

                    // ----- Carazo (códigos 41 al 48) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Jinotepe", Code = 41, DepartmentId = deptCarazo.Id, IsActive = true },
                    new Municipality { Name = "Diriamba", Code = 42, DepartmentId = deptCarazo.Id, IsActive = true },
                    new Municipality { Name = "San Marcos", Code = 43, DepartmentId = deptCarazo.Id, IsActive = true },
                    new Municipality { Name = "Santa Teresa", Code = 44, DepartmentId = deptCarazo.Id, IsActive = true },
                    new Municipality { Name = "Dolores", Code = 45, DepartmentId = deptCarazo.Id, IsActive = true },
                    new Municipality { Name = "La Paz de Carazo", Code = 46, DepartmentId = deptCarazo.Id, IsActive = true },
                    new Municipality { Name = "El Rosario", Code = 47, DepartmentId = deptCarazo.Id, IsActive = true },
                    new Municipality { Name = "La Conquista", Code = 48, DepartmentId = deptCarazo.Id, IsActive = true }
                });

                    // ----- Chinandega (códigos 81 al 93) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Chinandega", Code = 81, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "Corinto", Code = 82, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "El Realejo", Code = 83, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "Chichigalpa", Code = 84, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "Posoltega", Code = 85, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "El Viejo", Code = 86, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "Puerto Morazán", Code = 87, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "Somotillo", Code = 88, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "Villa Nueva", Code = 89, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "Santo Tomás del Norte", Code = 90, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "Cinco Pinos", Code = 91, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "San Francisco del Norte", Code = 92, DepartmentId = deptChinandega.Id, IsActive = true },
                    new Municipality { Name = "San Pedro del Norte", Code = 93, DepartmentId = deptChinandega.Id, IsActive = true }
                });

                    // ----- Chontales (códigos 121 al 130) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Juigalpa", Code = 121, DepartmentId = deptChontales.Id, IsActive = true },
                    new Municipality { Name = "Acoyapa", Code = 122, DepartmentId = deptChontales.Id, IsActive = true },
                    new Municipality { Name = "Santo Tomás", Code = 123, DepartmentId = deptChontales.Id, IsActive = true },
                    new Municipality { Name = "Villa Sandino", Code = 124, DepartmentId = deptChontales.Id, IsActive = true },
                    new Municipality { Name = "San Pedro de Lóvago", Code = 125, DepartmentId = deptChontales.Id, IsActive = true },
                    new Municipality { Name = "La Libertad", Code = 126, DepartmentId = deptChontales.Id, IsActive = true },
                    new Municipality { Name = "Santo Domingo", Code = 127, DepartmentId = deptChontales.Id, IsActive = true },
                    new Municipality { Name = "Comalapa", Code = 128, DepartmentId = deptChontales.Id, IsActive = true },
                    new Municipality { Name = "San Francisco de Cuapa", Code = 129, DepartmentId = deptChontales.Id, IsActive = true },
                    new Municipality { Name = "El Coral", Code = 130, DepartmentId = deptChontales.Id, IsActive = true }
                });

                    // ----- Estelí (códigos 161 al 166) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Estelí", Code = 161, DepartmentId = deptEsteli.Id, IsActive = true },
                    new Municipality { Name = "Pueblo Nuevo", Code = 162, DepartmentId = deptEsteli.Id, IsActive = true },
                    new Municipality { Name = "Condega", Code = 163, DepartmentId = deptEsteli.Id, IsActive = true },
                    new Municipality { Name = "San Juan de Limay", Code = 164, DepartmentId = deptEsteli.Id, IsActive = true },
                    new Municipality { Name = "La Trinidad", Code = 165, DepartmentId = deptEsteli.Id, IsActive = true },
                    new Municipality { Name = "San Nicolás", Code = 166, DepartmentId = deptEsteli.Id, IsActive = true }
                });

                    // ----- Granada (códigos 201 al 204) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Granada", Code = 201, DepartmentId = deptGranada.Id, IsActive = true },
                    new Municipality { Name = "Nandaime", Code = 202, DepartmentId = deptGranada.Id, IsActive = true },
                    new Municipality { Name = "Diriomo", Code = 203, DepartmentId = deptGranada.Id, IsActive = true },
                    new Municipality { Name = "Diriá", Code = 204, DepartmentId = deptGranada.Id, IsActive = true }
                });

                    // ----- Jinotega (códigos 241 al 247) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Jinotega", Code = 241, DepartmentId = deptJinotega.Id, IsActive = true },
                    new Municipality { Name = "San Rafael del Norte", Code = 242, DepartmentId = deptJinotega.Id, IsActive = true },
                    new Municipality { Name = "San Sebastián de Yalí", Code = 243, DepartmentId = deptJinotega.Id, IsActive = true },
                    new Municipality { Name = "La Concordia", Code = 244, DepartmentId = deptJinotega.Id, IsActive = true },
                    new Municipality { Name = "San José de Bocay", Code = 245, DepartmentId = deptJinotega.Id, IsActive = true },
                    new Municipality { Name = "El Cuá", Code = 246, DepartmentId = deptJinotega.Id, IsActive = true },
                    new Municipality { Name = "Santa María de Pantasma", Code = 247, DepartmentId = deptJinotega.Id, IsActive = true }
                });

                    // ----- León (códigos 281 al 291) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "León", Code = 281, DepartmentId = deptLeon.Id, IsActive = true },
                    new Municipality { Name = "El Jicaral", Code = 283, DepartmentId = deptLeon.Id, IsActive = true },
                    new Municipality { Name = "La Paz Centro", Code = 284, DepartmentId = deptLeon.Id, IsActive = true },
                    new Municipality { Name = "Santa Rosa del Peñón", Code = 285, DepartmentId = deptLeon.Id, IsActive = true },
                    new Municipality { Name = "Quezalguaque", Code = 286, DepartmentId = deptLeon.Id, IsActive = true },
                    new Municipality { Name = "Nagarote", Code = 287, DepartmentId = deptLeon.Id, IsActive = true },
                    new Municipality { Name = "El Sauce", Code = 288, DepartmentId = deptLeon.Id, IsActive = true },
                    new Municipality { Name = "Achuapa", Code = 289, DepartmentId = deptLeon.Id, IsActive = true },
                    new Municipality { Name = "Telica", Code = 290, DepartmentId = deptLeon.Id, IsActive = true },
                    new Municipality { Name = "Larreynaga (Malpaisillo)", Code = 291, DepartmentId = deptLeon.Id, IsActive = true }
                });

                    // ----- Madriz (códigos 321 al 329) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Somoto", Code = 321, DepartmentId = deptMadriz.Id, IsActive = true },
                    new Municipality { Name = "Telpaneca", Code = 322, DepartmentId = deptMadriz.Id, IsActive = true },
                    new Municipality { Name = "San Juan del Río Coco", Code = 323, DepartmentId = deptMadriz.Id, IsActive = true },
                    new Municipality { Name = "Palacagüina", Code = 324, DepartmentId = deptMadriz.Id, IsActive = true },
                    new Municipality { Name = "Yalagüina", Code = 325, DepartmentId = deptMadriz.Id, IsActive = true },
                    new Municipality { Name = "Totogalpa", Code = 326, DepartmentId = deptMadriz.Id, IsActive = true },
                    new Municipality { Name = "San Lucas", Code = 327, DepartmentId = deptMadriz.Id, IsActive = true },
                    new Municipality { Name = "Las Sabanas", Code = 328, DepartmentId = deptMadriz.Id, IsActive = true },
                    new Municipality { Name = "San José de Cusmapa", Code = 329, DepartmentId = deptMadriz.Id, IsActive = true }
                });

                    // ----- Masaya (códigos 401 al 409) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Masaya", Code = 401, DepartmentId = deptMasaya.Id, IsActive = true },
                    new Municipality { Name = "Nindirí", Code = 402, DepartmentId = deptMasaya.Id, IsActive = true },
                    new Municipality { Name = "Tisma", Code = 403, DepartmentId = deptMasaya.Id, IsActive = true },
                    new Municipality { Name = "Catarina", Code = 404, DepartmentId = deptMasaya.Id, IsActive = true },
                    new Municipality { Name = "San Juan de Oriente", Code = 405, DepartmentId = deptMasaya.Id, IsActive = true },
                    new Municipality { Name = "Niquinohomo", Code = 406, DepartmentId = deptMasaya.Id, IsActive = true },
                    new Municipality { Name = "Nandasmo", Code = 407, DepartmentId = deptMasaya.Id, IsActive = true },
                    new Municipality { Name = "Masatepe", Code = 408, DepartmentId = deptMasaya.Id, IsActive = true },
                    new Municipality { Name = "La Concepción", Code = 409, DepartmentId = deptMasaya.Id, IsActive = true }
                });

                    // ----- Matagalpa (códigos 441 al 454) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Matagalpa", Code = 441, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "San Ramón", Code = 442, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Matiguás", Code = 443, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Muy Muy", Code = 444, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Esquipulas", Code = 445, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "San Dionisio", Code = 446, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "San Isidro", Code = 447, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Sébaco", Code = 448, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Ciudad Darío", Code = 449, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Terrabona", Code = 450, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Río Blanco", Code = 451, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Tuma-La Dalia", Code = 452, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Rancho Grande", Code = 453, DepartmentId = deptMatagalpa.Id, IsActive = true },
                    new Municipality { Name = "Waslala", Code = 454, DepartmentId = deptMatagalpa.Id, IsActive = true }
                });

                    // ----- Nueva Segovia (códigos 481 al 493) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Ocotal", Code = 481, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Santa María", Code = 482, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Macuelizo", Code = 483, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Dipilto", Code = 484, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Ciudad Antigua", Code = 485, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Mozonte", Code = 486, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "San Fernando", Code = 487, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "El Jícaro", Code = 488, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Jalapa", Code = 489, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Murra", Code = 490, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Quilalí", Code = 491, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Wiwilí", Code = 492, DepartmentId = deptNuevaSegovia.Id, IsActive = true },
                    new Municipality { Name = "Wiwilí de Nueva Segovia", Code = 493, DepartmentId = deptNuevaSegovia.Id, IsActive = true }
                });

                    // ----- Río San Juan (códigos 521 al 526) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "San Carlos", Code = 521, DepartmentId = deptRioSanJuan.Id, IsActive = true },
                    new Municipality { Name = "El Castillo", Code = 522, DepartmentId = deptRioSanJuan.Id, IsActive = true },
                    new Municipality { Name = "San Miguelito", Code = 523, DepartmentId = deptRioSanJuan.Id, IsActive = true },
                    new Municipality { Name = "Morrito", Code = 524, DepartmentId = deptRioSanJuan.Id, IsActive = true },
                    new Municipality { Name = "San Juan del Norte", Code = 525, DepartmentId = deptRioSanJuan.Id, IsActive = true },
                    new Municipality { Name = "El Almendro", Code = 526, DepartmentId = deptRioSanJuan.Id, IsActive = true }
                });

                    // ----- Rivas (códigos 561 al 570) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Rivas", Code = 561, DepartmentId = deptRivas.Id, IsActive = true },
                    new Municipality { Name = "San Jorge", Code = 562, DepartmentId = deptRivas.Id, IsActive = true },
                    new Municipality { Name = "Buenos Aires", Code = 563, DepartmentId = deptRivas.Id, IsActive = true },
                    new Municipality { Name = "Potosí", Code = 564, DepartmentId = deptRivas.Id, IsActive = true },
                    new Municipality { Name = "Belén", Code = 565, DepartmentId = deptRivas.Id, IsActive = true },
                    new Municipality { Name = "Tola", Code = 566, DepartmentId = deptRivas.Id, IsActive = true },
                    new Municipality { Name = "San Juan del Sur", Code = 567, DepartmentId = deptRivas.Id, IsActive = true },
                    new Municipality { Name = "Cárdenas", Code = 568, DepartmentId = deptRivas.Id, IsActive = true },
                    new Municipality { Name = "Moyogalpa", Code = 569, DepartmentId = deptRivas.Id, IsActive = true },
                    new Municipality { Name = "Altagracia", Code = 570, DepartmentId = deptRivas.Id, IsActive = true }
                });

                    // ----- RACCN (códigos 607 al 615) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Puerto Cabezas", Code = 607, DepartmentId = deptRACCN.Id, IsActive = true },
                    new Municipality { Name = "Waspán", Code = 608, DepartmentId = deptRACCN.Id, IsActive = true },
                    new Municipality { Name = "Siuna", Code = 610, DepartmentId = deptRACCN.Id, IsActive = true },
                    new Municipality { Name = "Bonanza", Code = 611, DepartmentId = deptRACCN.Id, IsActive = true },
                    new Municipality { Name = "Rosita", Code = 612, DepartmentId = deptRACCN.Id, IsActive = true },
                    new Municipality { Name = "Bocana de Paiwas", Code = 615, DepartmentId = deptRACCN.Id, IsActive = true }
                });

                    // ----- RACCS (códigos 601 al 628) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Bluefields", Code = 601, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "Corn Island", Code = 602, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "El Rama", Code = 603, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "Muelle de los Bueyes", Code = 604, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "La Cruz de Río Grande", Code = 605, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "Prinzapolka", Code = 606, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "Nueva Guinea", Code = 616, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "Tortuguero", Code = 619, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "Kukra Hill", Code = 624, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "Laguna de Perlas", Code = 626, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "Desembocadura de Río Grande", Code = 627, DepartmentId = deptRACCS.Id, IsActive = true },
                    new Municipality { Name = "El Ayote", Code = 628, DepartmentId = deptRACCS.Id, IsActive = true }
                });

                    // ----- Boaco (códigos 361 al 366) -----
                    municipalities.AddRange(new[]
                    {
                    new Municipality { Name = "Boaco", Code = 361, DepartmentId = deptBoaco.Id, IsActive = true },
                    new Municipality { Name = "Camoapa", Code = 362, DepartmentId = deptBoaco.Id, IsActive = true },
                    new Municipality { Name = "Santa Lucía", Code = 363, DepartmentId = deptBoaco.Id, IsActive = true },
                    new Municipality { Name = "San José de los Remates", Code = 364, DepartmentId = deptBoaco.Id, IsActive = true },
                    new Municipality { Name = "San Lorenzo", Code = 365, DepartmentId = deptBoaco.Id, IsActive = true },
                    new Municipality { Name = "Teustepe", Code = 366, DepartmentId = deptBoaco.Id, IsActive = true }
                });

                    await context.Municipalities.AddRangeAsync(municipalities);
                    await context.SaveChangesAsync();

                    Console.WriteLine($"✅ Ubicación sembrada: {await context.Departments.CountAsync()} departamentos, {await context.Municipalities.CountAsync()} municipios (con códigos de cédula).");
                }
            }
        }
}
