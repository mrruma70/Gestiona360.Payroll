-- =====================================================================
-- REPORTE: Aporte INATEC Mensual (2% sobre nómina bruta)
-- PARÁMETROS: @Year, @Month, @CompanyId
-- =====================================================================
SELECT 
    c.LegalName AS Empresa,
    c.TaxId AS RUC,
    FORMAT(pp.StartDate, 'yyyy-MM') AS Periodo,
    SUM(pr.GrossIncome) AS NominaBrutaTotal,
    SUM(pr.GrossIncome) * 0.02 AS AporteINATEC
FROM dbo.PayrollRecords pr
INNER JOIN dbo.PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
INNER JOIN dbo.PayrollGroups pg ON pg.Id = pp.PayrollGroupId
INNER JOIN dbo.Companies c ON c.Id = pg.CompanyId
WHERE YEAR(pp.StartDate) = @Year
  AND MONTH(pp.StartDate) = @Month
  AND pg.CompanyId = @CompanyId
  AND pr.Status = 'Closed'
GROUP BY c.LegalName, c.TaxId, pp.StartDate;