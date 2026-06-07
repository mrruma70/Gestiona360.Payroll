SELECT 
    b.Code AS [Código],
    b.Name AS [Nombre Sucursal],
    b.Address AS [Dirección],
    b.City AS [Ciudad],
    b.Phone AS [Teléfono],
    ISNULL(cc.Name, 'Sin asignar') AS [Centro de Costo],
    ISNULL(cc.Code, '-') AS [Cód. Centro Costo],
    CASE WHEN b.IsActive = 1 THEN 'Activa' ELSE 'Inactiva' END AS [Estado],
    FORMAT(b.CreatedAt, 'dd/MM/yyyy') AS [Fecha Creación]
FROM Branches b
LEFT JOIN CostCenters cc ON cc.Id = b.DefaultCostCenterId
ORDER BY b.Code;