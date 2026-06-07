-- =====================================================================
-- REPORTE: Frecuencias de Pago
-- DESCRIPCIÓN: Catalogo de frecuencias de pago disponibles en el sistema.
-- PARÁMETROS: Sin parámetros, muestra todas las frecuencias activas.
-- =====================================================================
SELECT 
    Id AS [Id],
    Code AS [Código],
    Name AS [Nombre],
    DaysPerPeriod AS [Días/Período],
    PeriodsPerYear AS [Períodos/Año],
    Description AS [Descripción],
    CreatedAt AS [Fecha Creación],
    CASE WHEN IsActive = 1 THEN 'Sí' ELSE 'No' END AS [Activo]
FROM PayrollFrequencies
WHERE IsActive = 1
ORDER BY Code;