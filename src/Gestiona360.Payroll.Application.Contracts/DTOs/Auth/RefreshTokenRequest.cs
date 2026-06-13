using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Auth
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "El refresh token es obligatorio")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
