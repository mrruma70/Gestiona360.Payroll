-- =====================================================================
-- REPORTE: Resumen de Nómina por Sucursal
-- PARÁMETROS: @PayrollPeriodId
-- =====================================================================
SELECT 
    b.Code AS CodigoSucursal,
    b.Name AS Sucursal,
    cc.Code AS CentroCosto,
    COUNT(DISTINCT pr.EmployeeId) AS TotalEmpleados,
    SUM(pr.GrossIncome) AS SalarioBruto,
    SUM(pr.INSSWorker) AS TotalINSS_Trabajador,
    SUM(pr.INSSEmployer) AS TotalINSS_Empleador,
    SUM(pr.INATEC) AS TotalINATEC,
    SUM(pr.IR) AS TotalIR,
    SUM(pr.JudicialDeductions) AS TotalJudicial,
    SUM(pr.RecurringDeductionsTotal) AS TotalRecurrentes,
    SUM(pr.NetPay) AS TotalNeto
FROM dbo.PayrollRecords pr
INNER JOIN dbo.Employees e ON e.Id = pr.EmployeeId
INNER JOIN dbo.Branches b ON b.Id = e.BranchId
LEFT JOIN dbo.CostCenters cc ON cc.Id = b.DefaultCostCenterId  -- Ajusta según tu modelo
WHERE pr.PayrollPeriodId = @PayrollPeriodId
  AND pr.Status = 'Closed'
GROUP BY b.Code, b.Name, cc.Code
ORDER BY b.Code;