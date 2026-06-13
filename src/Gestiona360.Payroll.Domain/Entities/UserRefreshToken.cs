using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    /// <summary>
    /// Almacena refresh tokens válidos para cada usuario
    /// </summary>
    public class UserRefreshToken : BaseEntityGuid
    {
        /// <summary>
        /// Referencia al usuario
        /// </summary>
        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// El refresh token (string aleatorio + encriptado)
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// IP desde donde se generó el token
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// User Agent del navegador/cliente
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Fecha de expiración del refresh token
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Si fue revocado (logout)
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Fecha en que fue revocado
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// Razón de revocación
        /// </summary>
        public string? RevocationReason { get; set; }

        /// <summary>
        /// Es válido si no está expirado y no está revocado
        /// </summary>
        public bool IsValid => DateTime.UtcNow < ExpiresAt && !IsRevoked;
    }
}
