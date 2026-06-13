using MediatR;
using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;

namespace Gestiona360.Payroll.Application.Features.Auth.Commands
{
    public class RefreshTokenCommand : IRequest<LoginResponse>
    {
        public RefreshTokenCommand(string refreshToken, string? ipAddress = null)
        {
            RefreshToken = refreshToken;
            IpAddress = ipAddress;
        }

        public string RefreshToken { get; set; }
        public string? IpAddress { get; set; }
    }
}
