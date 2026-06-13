using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    /// <summary>
    /// DTO para filtros y paginación del lado del servidor en el listado de Acciones de Personal.
    /// Se usa con MudDataGrid ServerData.
    /// </summary>
    public class PersonalActionFilterDto
    {
        // ═══════════════════════════════════════════════════════════════
        // FILTROS PRINCIPALES
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// ID del período de nómina (OBLIGATORIO).
        /// Todas las consultas deben filtrar por período para respetar el principio
        /// "todo se basa en el Grupo de Nómina que define frecuencia/período".
        /// </summary>
        [Required(ErrorMessage = "El período de nómina es obligatorio")]
        public Guid PayrollPeriodId { get; set; }

        /// <summary>
        /// ID del grupo de nómina (opcional, para filtros adicionales).
        /// </summary>
        public Guid? PayrollGroupId { get; set; }

        /// <summary>
        /// Texto de búsqueda libre.
        /// Busca en: Employee.FirstName, Employee.LastName, Employee.Identification, 
        /// Employee.Code, BatchReference, CausalDescription.
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Filtro por tipo de acción (ej: "SalaryChange", "Suspension").
        /// Null = todos los tipos.
        /// </summary>
        public string? ActionType { get; set; }

        /// <summary>
        /// Filtro por estado (ej: "Pending", "Executed").
        /// Null = todos los estados.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filtro por referencia de lote (para ver todas las acciones de un evento masivo).
        /// </summary>
        public string? BatchReference { get; set; }

        /// <summary>
        /// Filtro por empleado específico.
        /// </summary>
        public Guid? EmployeeId { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // PAGINACIÓN
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Número de página (1-based)</summary>
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0")]
        public int PageNumber { get; set; } = 1;

        /// <summary>Cantidad de registros por página</summary>
        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        public int PageSize { get; set; } = 15;

        // ═══════════════════════════════════════════════════════════════
        // ORDENAMIENTO
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Campo por el cual ordenar (ej: "EffectiveDate", "Status")</summary>
        public string SortBy { get; set; } = "EffectiveDate";

        /// <summary>Dirección del ordenamiento</summary>
        public bool SortDescending { get; set; } = true;
    }
}
