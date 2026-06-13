using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo es inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Código de empresa/tenant (opcional, para multi-tenancy futuro)
        /// </summary>
        [StringLength(50)]
        public string? TenantCode { get; set; }
    }
}
