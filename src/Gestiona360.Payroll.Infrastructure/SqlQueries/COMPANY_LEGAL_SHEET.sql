SELECT 
    LegalName AS [Nombre Legal],
    CommercialName AS [Nombre Comercial],
    TaxId AS [RUC/NIT],
    INSSEmployerCode AS [Código Patronal INSS],
    EconomicActivityCode AS [Código CIIU],
    LegalRepresentative AS [Representante Legal],
    LegalRepresentativeId AS [Cédula Rep. Legal],
    Address AS [Dirección Fiscal],
    City AS [Ciudad],
    Department AS [Departamento],
    Phone AS [Teléfono],
    Email AS [Correo Electrónico],
    DefaultCurrency AS [Moneda Base],
    TotalActiveEmployees AS [Empleados Activos],
    CASE WHEN DefaultIsZoneFranca = 1 THEN 'Sí' ELSE 'No' END AS [Zona Franca],
    FORMAT(CreatedAt, 'dd/MM/yyyy') AS [Fecha Constitución]
FROM Companies
WHERE IsActive = 1;