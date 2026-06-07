using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;

namespace Gestiona360.Payroll.Infrastructure.Reporting
{
    /// <summary>
    /// Catálogo estático de reportes disponibles.
    /// En el futuro puede moverse a base de datos si se requiere edición dinámica.
    /// </summary>
    public static class ReportCatalog
    {
        public static IReadOnlyList<ReportDefinitionDto> GetAll() => new[]
        {
        // ============================================
        // REPORTES DGI (Impuesto sobre la Renta)
        // ============================================
        new ReportDefinitionDto(
            Code: "DGI_IR_RETENTION_MONTHLY",
            Name: "Retenciones IR Mensual (DGI)",
            Description: "Reporte mensual de retenciones de IR por empleado para la DGI.",
            DefaultFormat: "Excel",
            Parameters: new[]
            {
                new ReportParameterDefinitionDto("Year", "int", "Año", true, DateTime.UtcNow.Year.ToString()),
                new ReportParameterDefinitionDto("Month", "int", "Mes (1-12)", true, DateTime.UtcNow.Month.ToString())
            }
        ),

        new ReportDefinitionDto(
            Code: "DGI_IR_ANNUAL_CERTIFICATE",
            Name: "Certificado Anual IR por Empleado",
            Description: "Certificado anual de retenciones para cada empleado.",
            DefaultFormat: "Pdf",
            Parameters: new[]
            {
                new ReportParameterDefinitionDto("Year", "int", "Año", true, DateTime.UtcNow.Year.ToString()),
                new ReportParameterDefinitionDto("EmployeeId", "guid", "Empleado (opcional)", false, null)
            }
        ),

        // ============================================
        // REPORTES INSS
        // ============================================
        new ReportDefinitionDto(
            Code: "INSS_NOMINAL_MONTHLY",
            Name: "Planilla Nominal INSS Mensual",
            Description: "Reporte oficial de aportes INSS para presentación mensual.",
            DefaultFormat: "Excel",
            Parameters: new[]
            {
                new ReportParameterDefinitionDto("Year", "int", "Año", true, DateTime.UtcNow.Year.ToString()),
                new ReportParameterDefinitionDto("Month", "int", "Mes", true, DateTime.UtcNow.Month.ToString())
            }
        ),

        // ============================================
        // REPORTES INATEC
        // ============================================
        new ReportDefinitionDto(
            Code: "INATEC_MONTHLY",
            Name: "Aporte INATEC Mensual",
            Description: "Reporte del 2% sobre nómina bruta para INATEC.",
            DefaultFormat: "Excel",
            Parameters: new[]
            {
                new ReportParameterDefinitionDto("Year", "int", "Año", true, DateTime.UtcNow.Year.ToString()),
                new ReportParameterDefinitionDto("Month", "int", "Mes", true, DateTime.UtcNow.Month.ToString())
            }
        ),

        // ============================================
        // REPORTES INTERNOS / CONTABILIDAD
        // ============================================
        new ReportDefinitionDto(
            Code: "PAYROLL_SUMMARY_BY_BRANCH",
            Name: "Resumen de Nómina por Sucursal",
            Description: "Consolidado de nómina agrupado por sucursal y centro de costo.",
            DefaultFormat: "Excel",
            Parameters: new[]
            {
                new ReportParameterDefinitionDto("PayrollPeriodId", "guid", "Período de Nómina", true, null)
            }
        ),

        new ReportDefinitionDto(
            Code: "COLILLAS_PDF",
            Name: "Colillas de Pago (PDF)",
            Description: "Genera las colillas individuales de pago en PDF.",
            DefaultFormat: "Pdf",
            Parameters: new[]
            {
                new ReportParameterDefinitionDto("PayrollPeriodId", "guid", "Período", true, null),
                new ReportParameterDefinitionDto("EmployeeId", "guid", "Empleado (opcional)", false, null)
            }
        ),

        new ReportDefinitionDto(
            Code: "ACH_BAC_DISBURSEMENT",
            Name: "Archivo ACH - BAC Credomatic",
            Description: "Archivo de dispersión bancaria formato BAC.",
            DefaultFormat: "Csv",
            Parameters: new[]
            {
                new ReportParameterDefinitionDto("PayrollPeriodId", "guid", "Período", true, null)
            }
        ),

        // ============================================
        // REPORTES DE CATÁLOGOS (Frecuencias, Bancos, etc.)
        // ============================================
        new ReportDefinitionDto(
            Code: "PAYROLL_FREQUENCIES",
            Name: "Frecuencias de Nómina",
            Description: "Listado completo de frecuencias de pago configuradas en el sistema.",
            DefaultFormat: "Excel",
            Parameters: Array.Empty<ReportParameterDefinitionDto>()  // No requiere parámetros
        ),

        new ReportDefinitionDto(
            Code: "BANKS_CATALOG",
            Name: "Catálogo de Bancos",
            Description: "Listado de bancos configurados para dispersión de nómina.",
            DefaultFormat: "Excel",
            Parameters: Array.Empty<ReportParameterDefinitionDto>()
        ),

        new ReportDefinitionDto(
            Code: "OCCUPATIONAL_RISKS",
            Name: "Riesgos Ocupacionales",
            Description: "Catálogo de riesgos ocupacionales con tasas INSS.",
            DefaultFormat: "Excel",
            Parameters: Array.Empty<ReportParameterDefinitionDto>()
        ),

        // ============================================
        // REPORTES DE FICHAS Y LISTADOS
        // ============================================
        new ReportDefinitionDto(
            Code: "COMPANY_LEGAL_SHEET",
            Name: "Ficha Legal de la Empresa",
            Description: "Ficha legal con los datos de la empresa licenciataria.",
            DefaultFormat: "Pdf",
            Parameters: Array.Empty<ReportParameterDefinitionDto>()
        ),

        new ReportDefinitionDto(
            Code: "BRANCHES_LIST",
            Name: "Listado de Sucursales",
            Description: "Listado completo de sucursales con centros de costo.",
            DefaultFormat: "Excel",
            Parameters: Array.Empty<ReportParameterDefinitionDto>()
        ),

        // ============================================
        // REPORTES DE EMPLEADOS
        // ============================================
        new ReportDefinitionDto(
            Code: "EMPLOYEES_LIST",
            Name: "Listado de Empleados",
            Description: "Exportación de empleados con filtros dinámicos.",
            DefaultFormat: "Excel",
            Parameters: new[]
            {
                new ReportParameterDefinitionDto("Search", "string", "Búsqueda", false, null),
                new ReportParameterDefinitionDto("BranchId", "guid", "Sucursal", false, null),
                new ReportParameterDefinitionDto("ContractTypeId", "int", "Tipo de Contrato", false, null),
                new ReportParameterDefinitionDto("Status", "string", "Estado (active/inactive)", false, null),
                new ReportParameterDefinitionDto("JobPositionId", "guid", "Puesto", false, null)
            }
        )



    };


        public static ReportDefinitionDto? GetByCode(string code)
            => GetAll().FirstOrDefault(r => r.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }
}
