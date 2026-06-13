using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public LoginCommand(string email, string password, string? tenantCode = null)
        {
            Email = email;
            Password = password;
            TenantCode = tenantCode;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string? TenantCode { get; set; }

        /// <summary>
        /// IP address para auditoría (se llena en controller)
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent para auditoría (se llena en controller)
        /// </summary>
        public string? UserAgent { get; set; }
    }
}
