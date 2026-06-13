using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Auth
{
    public class LoginResponse
    {
        /// <summary>
        /// JWT Access Token (corta duración: 15-60 min)
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Refresh Token para renovar el Access Token
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Tiempo en segundos para expiración del Access Token
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Información del usuario autenticado
        /// </summary>
        public UserDto User { get; set; } = new();

        /// <summary>
        /// Tipo de token (siempre "Bearer")
        /// </summary>
        public string TokenType => "Bearer";
    }
}
