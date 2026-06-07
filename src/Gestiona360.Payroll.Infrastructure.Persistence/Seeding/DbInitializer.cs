// src/Gestiona360.Payroll.Infrastructure.Persistence/Seeding/DbInitializer.cs
using Microsoft.EntityFrameworkCore;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;


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

            // Configuración INSS
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

            // Configuración INATEC
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

            // Tramos IR
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

            // Salarios mínimos
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

            // Calendario de feriados
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

            // Conceptos de nómina
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

            // Bancos
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

            // Empresa
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

            // Sucursales
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

            // Centros de costo
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

            // Proveedor de salud
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

            // Grupo de nómina
            if (!await context.PayrollGroups.AnyAsync() && existingCompany != null && defaultFrequency != null)
            {
                var payrollGroup = new PayrollGroup
                {
                    Name = "Planilla Mensual Oficinas",
                    FrequencyId = defaultFrequency.Id,
                    CostCenterCode = "100-ADM",
                    ClosingDayRule = 28,
                    FirstPeriodStartDate = new DateTime(2026, 1, 1),
                    IsActive = true,
                    //CompanyId = existingCompany.Id
                };
                await context.PayrollGroups.AddAsync(payrollGroup);
                await context.SaveChangesAsync();
            }

            var payrollGroupEntity = await context.PayrollGroups.FirstOrDefaultAsync();

            // =================================================================
            // 3. CREAR EL RESTO DE PUESTOS (JobPositions) con GUID fijos
            // =================================================================

            // Diccionario para almacenar los IDs de los puestos recién creados (opcional, pero útil)
            var jobPositionIds = new Dictionary<string, Guid>();

            // Solo crear si no existen (evita duplicados)
            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "ADM-001"))
            {
                // Asistente Administrativo
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
                // Vendedor
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
                // Cajero
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

            // SEC-001
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

            // CON-001
            if (!await context.JobPositions.AnyAsync(jp => jp.Code == "CON-001"))
            {
                var conId = Guid.Parse("9F25FDE0-58BC-431E-B8F7-35295D045942");
                var conPosition = new JobPosition
                {
                    Id = conId,
                    Code = "CON-001",
                    Name = "Conductor",
                    Category = "Operativo",
                    OccupationalRiskId = 2, // Riesgo medio
                    MinimumWageId = servicioMinimum!.Id,
                    Description = "Transporte de personal o mercancías, mantenimiento básico del vehículo.",
                    IsTrustPosition = false,
                    RequiresLicense = false,
                    IsActive = true
                };
                await context.JobPositions.AddAsync(conPosition);
                jobPositionIds["CON-001"] = conId;
            }

            // ALM-001
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

            // SEG-001
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

            // LIM-001
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

            // TEC-001
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

            // RRHH-001
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

            // MKT-001
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

            // PROD-001
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

            // CAL-001
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

            // COM-001
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

            // REC-001
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
            Console.WriteLine("✅ Todos los puestos adicionales creados.");

            // =================================================================
            // 4. CREAR GRADOS (JobGrades) para cada puesto
            // =================================================================

            // --- Grados para Asistente Administrativo ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "ADM-JR"))
            {
                var admJr = new JobGrade
                {
                    JobPositionId = jobPositionIds["ADM-001"],
                    Code = "ADM-JR",
                    Name = "Asistente Administrativo Junior",
                    BaseSalaryMultiplier = 0.85m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(admJr);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "ADM-SR"))
            {
                var admSr = new JobGrade
                {
                    JobPositionId = jobPositionIds["ADM-001"],
                    Code = "ADM-SR",
                    Name = "Asistente Administrativo Senior",
                    BaseSalaryMultiplier = 1.15m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(admSr);
            }

            // --- Vendedor ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "VEN-JR"))
            {
                var venJr = new JobGrade
                {
                    JobPositionId = jobPositionIds["VEN-001"],
                    Code = "VEN-JR",
                    Name = "Vendedor Junior",
                    BaseSalaryMultiplier = 0.90m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(venJr);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "VEN-SR"))
            {
                var venSr = new JobGrade
                {
                    JobPositionId = jobPositionIds["VEN-001"],
                    Code = "VEN-SR",
                    Name = "Vendedor Senior",
                    BaseSalaryMultiplier = 1.20m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(venSr);
            }

            // --- Cajero ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "CAJ"))
            {
                var caj = new JobGrade
                {
                    JobPositionId = jobPositionIds["CAJ-001"],
                    Code = "CAJ",
                    Name = "Cajero",
                    BaseSalaryMultiplier = 1.00m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(caj);
            }

            // --- Secretaria ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "SEC-JR"))
            {
                var secJr = new JobGrade
                {
                    JobPositionId = jobPositionIds["SEC-001"],
                    Code = "SEC-JR",
                    Name = "Secretaria Junior",
                    BaseSalaryMultiplier = 0.90m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(secJr);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "SEC-SR"))
            {
                var secSr = new JobGrade
                {
                    JobPositionId = jobPositionIds["SEC-001"],
                    Code = "SEC-SR",
                    Name = "Secretaria Senior",
                    BaseSalaryMultiplier = 1.10m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(secSr);
            }

            // --- Conductor ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "CON"))
            {
                var con = new JobGrade
                {
                    JobPositionId = jobPositionIds["CON-001"],
                    Code = "CON",
                    Name = "Conductor",
                    BaseSalaryMultiplier = 1.00m,
                    RequiresLicense = true,
                    LicenseName = "Licencia de Conducir Clase B",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(con);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "CON-SUP"))
            {
                var conSup = new JobGrade
                {
                    JobPositionId = jobPositionIds["CON-001"],
                    Code = "CON-SUP",
                    Name = "Supervisor de Transporte",
                    BaseSalaryMultiplier = 1.30m,
                    RequiresLicense = true,
                    LicenseName = "Licencia de Conducir Clase B",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(conSup);
            }

            // --- Almacenista ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "ALM"))
            {
                var alm = new JobGrade
                {
                    JobPositionId = jobPositionIds["ALM-001"],
                    Code = "ALM",
                    Name = "Almacenista",
                    BaseSalaryMultiplier = 1.00m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(alm);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "ALM-SUP"))
            {
                var almSup = new JobGrade
                {
                    JobPositionId = jobPositionIds["ALM-001"],
                    Code = "ALM-SUP",
                    Name = "Supervisor de Almacén",
                    BaseSalaryMultiplier = 1.25m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(almSup);
            }

            // --- Guarda de Seguridad ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "SEG"))
            {
                var seg = new JobGrade
                {
                    JobPositionId = jobPositionIds["SEG-001"],
                    Code = "SEG",
                    Name = "Guarda de Seguridad",
                    BaseSalaryMultiplier = 1.00m,
                    RequiresLicense = true,
                    LicenseName = "Licencia de Vigilante",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(seg);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "SEG-JEF"))
            {
                var segJef = new JobGrade
                {
                    JobPositionId = jobPositionIds["SEG-001"],
                    Code = "SEG-JEF",
                    Name = "Jefe de Seguridad",
                    BaseSalaryMultiplier = 1.35m,
                    RequiresLicense = true,
                    LicenseName = "Licencia de Vigilante",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(segJef);
            }

            // --- Limpiador ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "LIM"))
            {
                var lim = new JobGrade
                {
                    JobPositionId = jobPositionIds["LIM-001"],
                    Code = "LIM",
                    Name = "Limpiador",
                    BaseSalaryMultiplier = 1.00m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(lim);
            }

            // --- Técnico en TI ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "TEC-JR"))
            {
                var tecJr = new JobGrade
                {
                    JobPositionId = jobPositionIds["TEC-001"],
                    Code = "TEC-JR",
                    Name = "Técnico TI Junior",
                    BaseSalaryMultiplier = 0.90m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(tecJr);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "TEC-SR"))
            {
                var tecSr = new JobGrade
                {
                    JobPositionId = jobPositionIds["TEC-001"],
                    Code = "TEC-SR",
                    Name = "Técnico TI Senior",
                    BaseSalaryMultiplier = 1.20m,
                    RequiresLicense = true,
                    LicenseName = "Certificación CompTIA o similar",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(tecSr);
            }

            // --- Asistente de RR.HH. ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "RRHH-JR"))
            {
                var rrhhJr = new JobGrade
                {
                    JobPositionId = jobPositionIds["RRHH-001"],
                    Code = "RRHH-JR",
                    Name = "Asistente RRHH Junior",
                    BaseSalaryMultiplier = 0.85m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(rrhhJr);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "RRHH-SR"))
            {
                var rrhhSr = new JobGrade
                {
                    JobPositionId = jobPositionIds["RRHH-001"],
                    Code = "RRHH-SR",
                    Name = "Asistente RRHH Senior",
                    BaseSalaryMultiplier = 1.15m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(rrhhSr);
            }

            // --- Coordinador de Marketing ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "MKT-JR"))
            {
                var mktJr = new JobGrade
                {
                    JobPositionId = jobPositionIds["MKT-001"],
                    Code = "MKT-JR",
                    Name = "Coordinador Marketing Junior",
                    BaseSalaryMultiplier = 0.95m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(mktJr);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "MKT-SR"))
            {
                var mktSr = new JobGrade
                {
                    JobPositionId = jobPositionIds["MKT-001"],
                    Code = "MKT-SR",
                    Name = "Coordinador Marketing Senior",
                    BaseSalaryMultiplier = 1.25m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(mktSr);
            }

            // --- Supervisor de Producción ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "PROD-JR"))
            {
                var prodJr = new JobGrade
                {
                    JobPositionId = jobPositionIds["PROD-001"],
                    Code = "PROD-JR",
                    Name = "Supervisor Producción Junior",
                    BaseSalaryMultiplier = 0.90m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(prodJr);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "PROD-SR"))
            {
                var prodSr = new JobGrade
                {
                    JobPositionId = jobPositionIds["PROD-001"],
                    Code = "PROD-SR",
                    Name = "Supervisor Producción Senior",
                    BaseSalaryMultiplier = 1.20m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(prodSr);
            }

            // --- Inspector de Calidad ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "CAL-JR"))
            {
                var calJr = new JobGrade
                {
                    JobPositionId = jobPositionIds["CAL-001"],
                    Code = "CAL-JR",
                    Name = "Inspector Calidad Junior",
                    BaseSalaryMultiplier = 0.90m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(calJr);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "CAL-SR"))
            {
                var calSr = new JobGrade
                {
                    JobPositionId = jobPositionIds["CAL-001"],
                    Code = "CAL-SR",
                    Name = "Inspector Calidad Senior",
                    BaseSalaryMultiplier = 1.15m,
                    RequiresLicense = true,
                    LicenseName = "Certificación en Control de Calidad",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(calSr);
            }

            // --- Comprador ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "COM-JR"))
            {
                var comJr = new JobGrade
                {
                    JobPositionId = jobPositionIds["COM-001"],
                    Code = "COM-JR",
                    Name = "Comprador Junior",
                    BaseSalaryMultiplier = 0.90m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(comJr);
            }
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "COM-SR"))
            {
                var comSr = new JobGrade
                {
                    JobPositionId = jobPositionIds["COM-001"],
                    Code = "COM-SR",
                    Name = "Comprador Senior",
                    BaseSalaryMultiplier = 1.20m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(comSr);
            }

            // --- Recepcionista ---
            if (!await context.JobGrades.AnyAsync(jg => jg.Code == "REC"))
            {
                var rec = new JobGrade
                {
                    JobPositionId = jobPositionIds["REC-001"],
                    Code = "REC",
                    Name = "Recepcionista",
                    BaseSalaryMultiplier = 1.00m,
                    RequiresLicense = false,
                    LicenseName = "",
                    IsActive = true
                };
                await context.JobGrades.AddAsync(rec);
            }

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Todos los grados adicionales creados.");

            // ✅ PUESTO (CON IsActive = true)
            //var jobPositionEntity = await context.JobPositions.FirstOrDefaultAsync(j => j.Code == "CONT");
            //if (jobPositionEntity == null && lowRisk != null && servicioMinimum != null)
            //{
            //    jobPositionEntity = new JobPosition
            //    {
            //        Code = "CONT",
            //        Name = "Contador",
            //        Category = "Administrativo",
            //        OccupationalRiskId = lowRisk.Id,
            //        MinimumWageId = servicioMinimum.Id,
            //        Description = "Encargado de contabilidad general",
            //        IsTrustPosition = false,
            //        RequiresLicense = false
            //    };
            //    await context.JobPositions.AddAsync(jobPositionEntity);
            //    await context.SaveChangesAsync();
            //    Console.WriteLine("✅ JobPosition creado: CONT");
            //}

            //// ✅ NIVEL (CON IsActive = true)
            //var jobGradeEntity = await context.JobGrades.FirstOrDefaultAsync(j => j.Code == "CONT-SR");
            //if (jobGradeEntity == null && jobPositionEntity != null)
            //{
            //    jobGradeEntity = new JobGrade
            //    {
            //        JobPositionId = jobPositionEntity.Id,
            //        Code = "CONT-SR",
            //        Name = "Contador Senior",
            //        BaseSalaryMultiplier = 1.0m,
            //        RequiresLicense = true,
            //        LicenseName = "CPA",
            //        IsActive = true
            //    };
            //    await context.JobGrades.AddAsync(jobGradeEntity);
            //    await context.SaveChangesAsync();
            //    Console.WriteLine("✅ JobGrade creado: CONT-SR");
            //}

            // =================================================================
            // EMPLEADO DEMO
            // =================================================================

            var existingDemoEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Code == "EMP-001");
            Employee? employee = null;

            if (existingDemoEmployee == null &&
                existingCompany != null &&
                branchMGA != null &&
                //jobGradeEntity != null &&
                healthProviderEntity != null &&
                lowRisk != null)
            {
                var contractIndefinido = await context.ContractTypes.FirstOrDefaultAsync(c => c.Name == "Indefinido");
                var bacBank = await context.Banks.FirstOrDefaultAsync(b => b.Code == "BAC");
                var inspectorGrade = await context.JobGrades.FirstOrDefaultAsync(jg => jg.Code == "CAL-JR");
             
                if (contractIndefinido != null)
                {
                    employee = new Employee
                    {
                        Code = "EMP-001",
                        Identification = "001-010285-1234K",
                        FirstName = "Juan",
                        LastName = "Pérez",
                        Email = "juan.perez@distribuidora.com.ni",
                        Phone = "+505 8888 1234",
                        HireDate = new DateTime(2025, 1, 15),
                        CompanyId = existingCompany.Id,
                        BranchId = branchMGA.Id,
                        ContractTypeId = contractIndefinido.Id,
                        JobGradeId = inspectorGrade?.Id,
                        HealthProviderId = healthProviderEntity.Id,
                        OccupationalRiskId = lowRisk.Id,
                        BaseSalary = 16000.00m,

                        // Datos bancarios
                        BankId = bacBank?.Id,
                        BankAccountNumber = bacBank != null ? "123456789" : null,
                        BankAccountType = bacBank != null ? "Ahorro" : null,

                        // Datos fiscales
                        NOINSS = "001-010285-1234K",
                        NORUC = "RUC123456789",

                        // Estado y configuración
                        IsActive = true,
                        EmploymentStatus = EmploymentStatus.Active,
                        IsTrustEmployee = false,

                        // Campos opcionales
                        PhotoUrl = null,
                        IdFrontUrl = null,
                        IdBackUrl = null,
                        PreviousEmployeeId = null,
                        BenefitsInKindDescription = null,
                        BenefitsInKindValue = null,
                        MitrabAuthorizationNumber = null,
                        Nationality = null,
                        Notes = null,
                        ProbationStartDate = null,
                        ProbationEndDate = null,
                        SuspensionStartDate = null,
                        SuspensionEndDate = null,
                        SuspensionJustification = null,
                        WorkPermitNumber = null,
                        WorkPermitExpirationDate = null,

                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Employees.AddAsync(employee);
                    await context.SaveChangesAsync();
                    Console.WriteLine("✅ Empleado demo creado exitosamente: EMP-001");
                }
                else
                {
                    Console.WriteLine("⚠️ No se encontró el tipo de contrato 'Indefinido'");
                }
            }
            else if (existingDemoEmployee != null)
            {
                employee = existingDemoEmployee;
                Console.WriteLine("ℹ️ Empleado demo ya existe: EMP-001");
            }
            else
            {
                Console.WriteLine("⚠️ No se pueden cumplir las dependencias para crear el empleado demo");
                Console.WriteLine($"   - existingCompany: {existingCompany != null}");
                Console.WriteLine($"   - branchMGA: {branchMGA != null}");
                //Console.WriteLine($"   - jobGradeEntity: {jobGradeEntity != null}");
                Console.WriteLine($"   - healthProviderEntity: {healthProviderEntity != null}");
                Console.WriteLine($"   - lowRisk: {lowRisk != null}");
            }

            if (employee == null)
            {
                employee = await context.Employees.FirstOrDefaultAsync();
            }

            // Período de nómina abierto
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

            // =================================================================
            // 3. DATOS COMPLEMENTARIOS
            // =================================================================

            // Turnos y horarios
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

            // Mapeo de conceptos a cuentas contables
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

            // Saldo de vacaciones inicial
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

            // Provisión de aguinaldo
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

            // Provisión de indemnización
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

            // Asignación de concepto de préstamo
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

            await context.SaveChangesAsync();
        }
    }
}

