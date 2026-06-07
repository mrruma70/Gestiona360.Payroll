-- =====================================================================
-- REPORTE: Planilla Nominal INSS Mensual
-- PARÁMETROS: @Year, @Month, @CompanyId
-- =====================================================================
SELECT 
    c.INSSEmployerCode AS CodigoPatronal,
    e.Identification AS Cedula,
    CONCAT(e.LastName, ', ', e.FirstName) AS NombreEmpleado,
    pr.GrossIncome AS SalarioBruto,
    CASE WHEN pr.GrossIncome > @MaxSalaryCap THEN @MaxSalaryCap ELSE pr.GrossIncome END AS SalarioCotizable,
    pr.INSSWorker AS AporteTrabajador,
    CASE WHEN pr.GrossIncome > @MaxSalaryCap 
         THEN @MaxSalaryCap * @EmployerRate / 100 
         ELSE pr.GrossIncome * @EmployerRate / 100 END AS AporteEmpleador
FROM dbo.PayrollRecords pr
INNER JOIN dbo.Employees e ON e.Id = pr.EmployeeId
INNER JOIN dbo.PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
INNER JOIN dbo.PayrollGroups pg ON pg.Id = pp.PayrollGroupId
INNER JOIN dbo.Companies c ON c.Id = pg.CompanyId
WHERE YEAR(pp.StartDate) = @Year
  AND MONTH(pp.StartDate) = @Month
  AND pg.CompanyId = @CompanyId
  AND pr.Status = 'Closed'
ORDER BY e.LastName;