using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO para exportación de empleados a Excel.
    /// Organizado por grupos del maestro de empleados.
    /// </summary>
    public class EmployeeExportDto
    {
        // GRUPO 1: IDENTIFICACIÓN BÁSICA
        public string Codigo { get; set; } = "";
        public string Cedula { get; set; } = "";
        public string? PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Sexo { get; set; }
        public string? EstadoCivil { get; set; }

        // GRUPO 2: CONTACTO Y DOMICILIO
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public string? Departamento { get; set; }
        public string? Municipio { get; set; }

        // GRUPO 3: CONTACTO DE EMERGENCIA
        public string? ContactoEmergencia { get; set; }
        public string? TelefonoEmergencia { get; set; }
        public string? Parentesco { get; set; }

        // GRUPO 4: DATOS FISCALES
        public string? RUC { get; set; }
        public string? INSS { get; set; }

        // GRUPO 5: DATOS LABORALES
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaPrimerIngreso { get; set; }
        public string? Puesto { get; set; }
        public string? Nivel { get; set; }
        public string? Sucursal { get; set; }
        public string? CodigoSucursal { get; set; }
        public string? TipoContrato { get; set; }
        public decimal SalarioBase { get; set; }
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        public string? GrupoNomina { get; set; }
        public string? ProveedorSalud { get; set; }

        // GRUPO 6: DATOS BANCARIOS
        public string? Banco { get; set; }
        public string? TipoCuenta { get; set; }
        public string? CuentaBancaria { get; set; }
        public string? Beneficiario { get; set; }

        // GRUPO 7: CONDICIONES ESPECIALES
        public int EmploymentStatus { get; set; }
        public bool Estado { get; set; }
        public bool EsConfianza { get; set; }
        public bool UsaRelojMarcas { get; set; }
        public DateTime? InicioPrueba { get; set; }
        public DateTime? FinPrueba { get; set; }

        // GRUPO 8: TRABAJADOR EXTRANJERO
        public string? Nacionalidad { get; set; }
        public string? PermisoTrabajo { get; set; }
        public DateTime? VencimientoPermiso { get; set; }

        // GRUPO 9: BENEFICIOS EN ESPECIE
        public decimal? ValorEspecie { get; set; }
        public string? DescripcionEspecie { get; set; }

        // GRUPO 10: SUSPENSIÓN
        public DateTime? InicioSuspension { get; set; }
        public DateTime? FinSuspension { get; set; }

        // GRUPO 11: REINGRESO
        public Guid? PreviousEmployeeId { get; set; }

        // GRUPO 12: NOTAS
        public string? Notes { get; set; }
    }
}
