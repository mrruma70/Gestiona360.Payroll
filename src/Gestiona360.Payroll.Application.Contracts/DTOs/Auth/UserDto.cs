using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public List<string> Roles { get; set; } = new();

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
