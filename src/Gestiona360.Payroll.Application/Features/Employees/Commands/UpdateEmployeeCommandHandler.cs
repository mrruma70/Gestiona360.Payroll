using Gestiona360.Payroll.Application.Contracts.Common.Validators;
using Gestiona360.Payroll.Application.Contracts.Requests;
using Gestiona360.Payroll.Application.Features.Employees.Commands;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateEmployeeCommandHandler(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (employee == null)
                throw new KeyNotFoundException($"Empleado con ID {request.Id} no encontrado.");

            // ═══════════════════════════════════════════════════════════════
            // VALIDACIONES DE CAMPOS OBLIGATORIOS
            // ═══════════════════════════════════════════════════════════════
            if (!request.BirthDate.HasValue)
                throw new InvalidOperationException("La fecha de nacimiento es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.MobilePhone))
                throw new InvalidOperationException("El número de celular es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Address))
                throw new InvalidOperationException("La dirección del domicilio es obligatoria.");

            if (!request.DepartmentId.HasValue)
                throw new InvalidOperationException("El departamento es obligatorio.");

            if (!request.MunicipalityId.HasValue)
                throw new InvalidOperationException("El municipio es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.EmergencyContactName))
                throw new InvalidOperationException("El nombre del contacto de emergencia es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.EmergencyContactPhone))
                throw new InvalidOperationException("El teléfono del contacto de emergencia es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.EmergencyContactRelationship))
                throw new InvalidOperationException("El parentesco del contacto de emergencia es obligatorio.");

            // ═══════════════════════════════════════════════════════════════
            // VALIDACIÓN DE CAMBIO DE CÉDULA (SOLO ADMIN)
            // ═══════════════════════════════════════════════════════════════
            // Normalizar ambas cédulas para comparar correctamente
            var cedulaActual = (employee.Identification ?? string.Empty).Trim().ToUpper();
            var cedulaNueva = (request.Identification ?? string.Empty).Trim().ToUpper();

            // Solo procesar si la cédula REALMENTE cambió
            if (!string.Equals(cedulaActual, cedulaNueva, StringComparison.Ordinal))
            {
                // La cédula cambió - validar que sea un cambio permitido

                // 1. Validar formato matemático de la nueva cédula
                if (!CedulaValidator.EsValida(cedulaNueva))
                    throw new InvalidOperationException(
                        $"La nueva cédula '{cedulaNueva}' no es válida según el algoritmo de verificación nicaragüense.");

                // 2. Verificar que NO exista OTRO empleado con esa cédula
                var empleadoConflictivo = await _context.Employees
                    .Where(e => e.Identification == cedulaNueva && e.Id != request.Id)
                    .Select(e => new { e.Id, e.Code, e.FirstName, e.LastName, e.IsActive })
                    .FirstOrDefaultAsync(cancellationToken);

                if (empleadoConflictivo != null)
                {
                    throw new InvalidOperationException(
                        $"Ya existe otro empleado con la cédula {cedulaNueva}.\n" +
                        $"Empleado conflictivo: {empleadoConflictivo.Code} - " +
                        $"{empleadoConflictivo.FirstName} {empleadoConflictivo.LastName} " +
                        $"({(empleadoConflictivo.IsActive ? "Activo" : "Inactivo")})\n" +
                        $"No se puede asignar la misma cédula a dos empleados diferentes.");
                }

                // 3. Si pasó todas las validaciones, actualizar la cédula
                employee.Identification = cedulaNueva;
            }
            //if (!string.IsNullOrWhiteSpace(request.Identification) &&
            //    !string.Equals(request.Identification, employee.Identification, StringComparison.OrdinalIgnoreCase))
            //{
            //    // Verificar que el usuario sea Admin (inyectar IHttpContextAccessor o similar)
            //    // Aquí va la validación de rol en backend

            //    // Validar formato y unicidad de la nueva cédula
            //    if (!CedulaValidator.EsValida(request.Identification))
            //        throw new InvalidOperationException("La nueva cédula no es válida según el algoritmo de verificación.");

            //    var existeOtroEmpleado = await _context.Employees
            //        .AnyAsync(e => e.Identification == request.Identification && e.Id != request.Id, cancellationToken);

            //    if (existeOtroEmpleado)
            //        throw new InvalidOperationException($"Ya existe otro empleado con la cédula {request.Identification}.");

            //    // Actualizar la cédula
            //    employee.Identification = request.Identification;
            //}

            // ═══════════════════════════════════════════════════════════════
            // VALIDAR DEPARTAMENTO Y MUNICIPIO
            // ═══════════════════════════════════════════════════════════════
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.IsActive, cancellationToken);
            if (department == null)
                throw new InvalidOperationException("El departamento seleccionado no existe o está inactivo.");

            var municipality = await _context.Municipalities
                .FirstOrDefaultAsync(m => m.Id == request.MunicipalityId && m.IsActive && m.DepartmentId == request.DepartmentId, cancellationToken);
            if (municipality == null)
                throw new InvalidOperationException("El municipio seleccionado no existe, está inactivo o no pertenece al departamento.");

            // ═══════════════════════════════════════════════════════════════
            // DATOS PERSONALES
            // ═══════════════════════════════════════════════════════════════
            employee.FirstName = request.FirstName;
            employee.SecondName = request.SecondName;
            employee.LastName = request.LastName;
            employee.SecondLastName = request.SecondLastName;
            employee.BirthDate = request.BirthDate;
            employee.Gender = request.Gender;
            employee.MaritalStatus = request.MaritalStatus;

            // ═══════════════════════════════════════════════════════════════
            // CONTACTO
            // ═══════════════════════════════════════════════════════════════
            employee.Email = request.Email;
            employee.Phone = request.Phone;
            employee.MobilePhone = request.MobilePhone;

            // ═══════════════════════════════════════════════════════════════
            // DOMICILIO
            // ═══════════════════════════════════════════════════════════════
            employee.Address = request.Address;
            employee.DepartmentId = request.DepartmentId;
            employee.MunicipalityId = request.MunicipalityId;

            // ═══════════════════════════════════════════════════════════════
            // CONTACTO DE EMERGENCIA
            // ═══════════════════════════════════════════════════════════════
            employee.EmergencyContactName = request.EmergencyContactName;
            employee.EmergencyContactPhone = request.EmergencyContactPhone;
            employee.EmergencyContactRelationship = request.EmergencyContactRelationship;

            // ═══════════════════════════════════════════════════════════════
            // DATOS FISCALES
            // ═══════════════════════════════════════════════════════════════
            employee.NORUC = request.NORUC;
            employee.NOINSS = request.NOINSS;

            // ═══════════════════════════════════════════════════════════════
            // ASIGNACIÓN LABORAL
            // ═══════════════════════════════════════════════════════════════
            employee.BranchId = request.BranchId;
            employee.PayrollGroupId = request.PayrollGroupId;
            employee.ContractTypeId = request.ContractTypeId;
            employee.BaseSalary = request.BaseSalary;

            // ═══════════════════════════════════════════════════════════════
            // CONDICIONES ESPECIALES
            // ═══════════════════════════════════════════════════════════════
            employee.IsTrustEmployee = request.IsTrustEmployee;
            employee.UsesTimeClock = request.UsesTimeClock;
            employee.BenefitsInKindValue = request.BenefitsInKindValue;
            employee.BenefitsInKindDescription = request.BenefitsInKindDescription;

            // ═══════════════════════════════════════════════════════════════
            // TRABAJADOR EXTRANJERO
            // ═══════════════════════════════════════════════════════════════
            employee.Nationality = request.Nationality;
            employee.WorkPermitNumber = request.WorkPermitNumber;
            employee.WorkPermitExpirationDate = request.WorkPermitExpirationDate;

            // ═══════════════════════════════════════════════════════════════
            // DATOS BANCARIOS (solo beneficiario)
            // ═══════════════════════════════════════════════════════════════
            employee.BankBeneficiaryName = request.BankBeneficiaryName;

            // ═══════════════════════════════════════════════════════════════
            // ESTADO Y NOTAS
            // ═══════════════════════════════════════════════════════════════
            employee.IsActive = request.IsActive;
            employee.Notes = request.Notes;

            // ═══════════════════════════════════════════════════════════════
            // IMÁGENES
            // ═══════════════════════════════════════════════════════════════
            employee.PhotoUrl = request.PhotoUrl;
            employee.IdFrontUrl = request.IdFrontUrl;
            employee.IdBackUrl = request.IdBackUrl;

            // ═══════════════════════════════════════════════════════════════
            // CONTRATACIÓN
            // ═══════════════════════════════════════════════════════════════
            employee.FirstHireDate = request.FirstHireDate;

            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}