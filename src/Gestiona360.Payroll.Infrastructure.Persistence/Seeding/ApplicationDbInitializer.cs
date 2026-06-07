using Gestiona360.Payroll.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Seeding
{
    public static class ApplicationDbInitializer
    {
        public static async Task InitializeIdentityAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ILogger logger)
        {
            try
            {
                // 1. Crear roles si no existen
                await SeedRolesAsync(roleManager);

                // 2. Crear usuario administrador y asignarle rol
                var adminUser = await SeedAdminUserAsync(userManager, context, logger);

                // 3. Sincronizar con el empleado demo (opcional)
                await SyncAdminWithEmployeeAsync(context, userManager, adminUser, logger);

                // Guardar cambios finales (si hubo actualizaciones de Employee)
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al inicializar la identidad y roles del sistema.");
                throw;
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roleNames = {
                "Administrador",
                "Planillero",
                "Contador",
                "GerenteRRHH",
                "Auditor",
                "Consulta"
            };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
        }

        private static async Task<ApplicationUser> SeedAdminUserAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger logger)
        {
            const string adminEmail = "admin@localhost";
            const string adminPassword = "Admin123!";

            var adminUser = await userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Administrador",
                    LastName = "del Sistema",
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"No se pudo crear el usuario administrador: {errors}");
                }
                logger.LogInformation("Usuario administrador creado correctamente.");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
            {
                await userManager.AddToRoleAsync(adminUser, "Administrador");
                logger.LogInformation("Rol 'Administrador' asignado al usuario admin.");
            }

            return adminUser;
        }

        private static async Task SyncAdminWithEmployeeAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ApplicationUser adminUser,
            ILogger logger)
        {
            // Buscar el empleado demo (creado previamente en DbInitializer)
            var demoEmployee = await context.Employees
                .FirstOrDefaultAsync(e => e.Email == "juan.perez@distribuidora.com.ni");

            if (demoEmployee != null && adminUser.EmployeeId == null)
            {
                adminUser.EmployeeId = demoEmployee.Id; // Guid?
                var result = await userManager.UpdateAsync(adminUser);
                if (result.Succeeded)
                {
                    logger.LogInformation("Usuario admin vinculado al empleado demo.");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogWarning($"No se pudo vincular el usuario admin con el empleado demo: {errors}");
                }
            }
        }
    }
}