using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Identity.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Gestiona360.Payroll.Application.Features.Auth.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
    {
        private readonly ITokenGenerationService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RefreshTokenCommandHandler(
            ITokenGenerationService tokenService,
            UserManager<ApplicationUser> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Obtener principal del token expirado para extraer el user ID
                var principal = _tokenService.GetPrincipalFromExpiredToken(request.RefreshToken);

                var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                    throw new SecurityTokenException("Token inválido");

                // 2. Validar que el refresh token sea válido
                var isValid = await _tokenService.ValidateRefreshTokenAsync(userId, request.RefreshToken);
                if (!isValid)
                    throw new SecurityTokenException("Refresh token inválido o expirado");

                // 3. Obtener usuario
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    throw new InvalidOperationException("Usuario no encontrado");

                // 4. Obtener roles
                var roles = await _userManager.GetRolesAsync(user);

                // 5. Generar nuevos tokens
                var (newAccessToken, newRefreshToken) = await _tokenService.GenerateTokensAsync(
                    user,
                    roles,
                    request.IpAddress);

                // 6. Revocar el refresh token anterior
                await _tokenService.RevokeRefreshTokenAsync(userId, request.RefreshToken, "Token refreshed");

                // 7. Retornar respuesta
                return new LoginResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresIn = 900,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email ?? "",
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Roles = roles.ToList(),
                        IsActive = true,
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Refresh token inválido: {ex.Message}", ex);
            }
        }
    }
}