using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    /// <summary>
    /// Resultado paginado del listado de Acciones de Personal.
    /// </summary>
    public class PersonalActionPagedResultDto
    {
        /// <summary>Lista de acciones de la página actual</summary>
        public List<PersonalActionListDto> Items { get; set; } = new();

        /// <summary>Total de registros que coinciden con el filtro</summary>
        public int TotalCount { get; set; }

        /// <summary>Página actual</summary>
        public int PageNumber { get; set; }

        /// <summary>Tamaño de página</summary>
        public int PageSize { get; set; }

        /// <summary>Total de páginas</summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>Indica si hay página anterior</summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>Indica si hay página siguiente</summary>
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
