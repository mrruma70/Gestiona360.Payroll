using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gestiona360.Payroll.Infrastructure.Identity.Services
{
    /// <summary>
    /// Servicio para generar y validar JWT tokens y refresh tokens
    /// </summary>
    public interface ITokenGenerationService
    {
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(
            ApplicationUser user,
            IList<string> roles,
            string? ipAddress = null,
            string? userAgent = null);

        Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);

        Task RevokeRefreshTokenAsync(Guid userId, string refreshToken, string reason = "Logout");

        string GenerateJwtToken(ApplicationUser user, IList<string> roles);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }

    public class TokenGenerationService : ITokenGenerationService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore<ApplicationUser> _userStore;

        public TokenGenerationService(
            IConfiguration configuration,
            ApplicationDbContext context,
            IUserStore<ApplicationUser> userStore)
        {
            _configuration = configuration;
            _context = context;
            _userStore = userStore;
        }

        /// <summary>
        /// Genera JWT access token y refresh token
        /// </summary>
        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(
            ApplicationUser user,
            IList<string> roles,
            string? ipAddress = null,
            string? userAgent = null)
        {
            // 1. Generar JWT corto
            var accessToken = GenerateJwtToken(user, roles);

            // 2. Generar refresh token largo
            var refreshToken = GenerateRefreshToken();

            // 3. Guardar refresh token en BD
            var tokenEntity = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7")),
                IsRevoked = false
            };

            _context.UserRefreshTokens.Add(tokenEntity);
            await _context.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        /// <summary>
        /// Valida que un refresh token sea válido
        /// </summary>
        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var storedToken = await _context.UserRefreshTokens
                .Where(rt => rt.UserId == userId && rt.Token == refreshToken)
                .FirstOrDefaultAsync();

            if (storedToken == null)
                return false;

            return storedToken.IsValid;
        }

        /// <summary>
        /// Revoca un refresh token (logout)
        /// </summary>
        public async Task RevokeRefreshTokenAsync(
            Guid userId,
            string refreshToken,
            string reason = "Logout")
        {
            var token = await _context.UserRefreshTokens
                .Where(rt => rt.UserId == userId && rt.Token == refreshToken)
                .FirstOrDefaultAsync();

            if (token != null)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevocationReason = reason;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Genera JWT token con claims del usuario
        /// </summary>
        public string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no configurada");
            var secretKey = Convert.FromBase64String(jwtKey); // ✅ Decodificar Base64

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Name, user.UserName ?? ""),
        new Claim("FirstName", user.FirstName ?? ""),
        new Claim("LastName", user.LastName ?? "")
    };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "15");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256)  // ✅ Usa HmacSha256 (sin "Signature")
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        /// <summary>
        /// Genera un refresh token aleatorio de 64 bytes (base64)
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Extrae principal del JWT expirado (para validar refresh)
        /// </summary>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no configurada");
            var secretKey = Convert.FromBase64String(jwtKey); // ✅ Decodificar Base64

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Token inválido");
            return principal;
        }
    }
}