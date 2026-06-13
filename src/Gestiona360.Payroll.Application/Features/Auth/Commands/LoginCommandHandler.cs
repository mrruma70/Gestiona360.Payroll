using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;
using Gestiona360.Payroll.Application.Contracts;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Identity.Services;


namespace Gestiona360.Payroll.Application.Features.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenGenerationService _tokenService;
        private readonly IApplicationDbContext _context;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            ITokenGenerationService tokenService,
            IApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar credenciales
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Credenciales inválidas");

            if (!user.EmailConfirmed)
                throw new InvalidOperationException("El correo no está confirmado");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Credenciales inválidas");

            if (!user.LockoutEnd.HasValue || user.LockoutEnd < DateTime.UtcNow)
            {
                // Usuario no está bloqueado
            }
            else
            {
                throw new InvalidOperationException("La cuenta está bloqueada");
            }

            // 2. Obtener roles del usuario
            var roles = await _userManager.GetRolesAsync(user);

            // 3. Generar tokens
            var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(
                user,
                roles,
                request.IpAddress,
                request.UserAgent);

            // 4. Construir response
            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 900, // 15 minutos en segundos
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList(),
                    IsActive = !user.LockoutEnabled,
                    CreatedAt = DateTime.UtcNow
                }
            };

            return response;
        }
    }
}
