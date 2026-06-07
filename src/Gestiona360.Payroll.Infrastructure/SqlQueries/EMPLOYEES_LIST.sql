-- =====================================================================
-- REPORTE: Listado de Empleados
-- PARÁMETROS: Todos opcionales (se pasan como NULL desde Dapper)
-- =====================================================================
SELECT 
    e.Code AS Codigo,
    e.Identification AS Cedula,
    e.FirstName AS Nombres,
    e.LastName AS Apellidos,
    e.Email AS Correo,
    e.Phone AS Telefono,
    e.HireDate AS FechaIngreso,
    CASE WHEN e.IsActive = 1 THEN 'Activo' ELSE 'Inactivo' END AS Estado,
    ISNULL(jp.Name, 'Sin puesto') AS Puesto,
    ISNULL(jg.Name, '') AS Nivel,
    ISNULL(b.Name, '') AS Sucursal,
    ISNULL(b.Code, '') AS CodigoSucursal,
    ISNULL(ct.Name, '') AS TipoContrato,
    e.BaseSalary AS SalarioBase
FROM dbo.Employees e
LEFT JOIN dbo.JobGrades jg ON jg.Id = e.JobGradeId
LEFT JOIN dbo.JobPositions jp ON jp.Id = jg.JobPositionId
LEFT JOIN dbo.Branches b ON b.Id = e.BranchId
LEFT JOIN dbo.ContractTypes ct ON ct.Id = e.ContractTypeId
WHERE 1 = 1
    AND (@Search = '' OR e.FirstName LIKE '%' + @Search + '%' OR e.LastName LIKE '%' + @Search + '%' OR e.Identification LIKE '%' + @Search + '%' OR e.Code LIKE '%' + @Search + '%')
    AND (@BranchId = '00000000-0000-0000-0000-000000000000' OR e.BranchId = @BranchId)
    AND (@ContractTypeId = 0 OR e.ContractTypeId = @ContractTypeId)
    AND (@Status = '' OR (@Status = 'active' AND e.IsActive = 1) OR (@Status = 'inactive' AND e.IsActive = 0))
    AND (@JobPositionId = '00000000-0000-0000-0000-000000000000' OR jg.JobPositionId = @JobPositionId)
ORDER BY e.FirstName, e.LastName;