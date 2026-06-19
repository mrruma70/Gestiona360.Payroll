using System.ComponentModel.DataAnnotations;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO Base para todos los catálogos del sistema
    /// Soporta cualquier tabla paramétrica de la nómina
    /// </summary>
    public class CatalogItemDto
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El código es requerido")]
        [StringLength(20, MinimumLength = 1,
            ErrorMessage = "El código debe tener entre 1 y 20 caracteres")]
        public string Code { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Value { get; set; } // Valor dinámico (porcentaje, monto, etc.)

        public bool IsActive { get; set; } = true;

        [Display(Name = "Creado")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Actualizado")]
        public DateTime? UpdatedAt { get; set; }

        // Métodos auxiliares
        public CatalogItemDto Clone()
        {
            return new CatalogItemDto
            {
                Id = this.Id,
                Name = this.Name,
                Code = this.Code,
                Description = this.Description,
                Value = this.Value,
                IsActive = this.IsActive,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt
            };
        }

    }

}
