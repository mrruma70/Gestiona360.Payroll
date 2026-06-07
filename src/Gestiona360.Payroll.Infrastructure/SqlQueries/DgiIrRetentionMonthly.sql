-- =====================================================================
-- REPORTE: Retenciones IR Mensual (DGI)
-- DESCRIPCIÓN: Consolidado mensual de retenciones por empleado
-- PARÁMETROS: @Year, @Month
-- =====================================================================
SELECT 
    e.Identification AS Cedula,
    CONCAT(e.FirstName, ' ', e.LastName) AS NombreEmpleado,
    pr.GrossIncome AS SalarioBruto,
    pr.INSSWorker AS AporteINSS,
    (pr.GrossIncome - pr.INSSWorker) AS RetaNeta,
    pr.IR AS IRRetenido,
    pp.StartDate AS FechaInicio,
    pp.EndDate AS FechaFin
FROM dbo.PayrollRecords pr
INNER JOIN dbo.Employees e ON e.Id = pr.EmployeeId
INNER JOIN dbo.PayrollPeriods pp ON pp.Id = pr.PayrollPeriodId
INNER JOIN dbo.PayrollGroups pg ON pg.Id = pp.PayrollGroupId
WHERE YEAR(pp.StartDate) = @Year
  AND MONTH(pp.StartDate) = @Month
  AND pg.CompanyId = @CompanyId
  AND pr.Status = 'Closed'
ORDER BY e.LastName, e.FirstName;