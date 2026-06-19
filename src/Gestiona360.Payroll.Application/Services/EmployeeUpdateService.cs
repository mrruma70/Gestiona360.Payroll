// src/Gestiona360.Payroll.Application/Services/EmployeeUpdateService.cs
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.Requests;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Application.Services;

/// <summary>
/// Servicio de aplicación responsable de mapear DTOs a entidades para actualizaciones.
/// Vive en Application porque conoce tanto el DTO como la estructura interna de la entidad.
/// La entidad Employee permanece pura y sin dependencias externas.
/// </summary>
public static class EmployeeUpdateService
{
    /// <summary>
    /// Aplica cambios desde un DTO a una entidad existente.
    /// NO modifica propiedades de solo lectura ni estados internos protegidos.
    /// </summary>
    public static void ApplyChanges(Employee employee, UpdateEmployeeRequest request)
    {
        ArgumentNullException.ThrowIfNull(employee);
        ArgumentNullException.ThrowIfNull(request);

        // Datos personales
        employee.FirstName = request.FirstName;
        employee.SecondName = request.SecondName;
        employee.LastName = request.LastName;
        employee.SecondLastName = request.SecondLastName;
        employee.BirthDate = request.BirthDate;
        employee.Gender = request.Gender;
        employee.MaritalStatus = request.MaritalStatus;

        // Contacto
        employee.Email = request.Email;
        employee.Phone = request.Phone;
        employee.MobilePhone = request.MobilePhone;

        // Domicilio
        employee.Address = request.Address;
        employee.DepartmentId = request.DepartmentId;
        employee.MunicipalityId = request.MunicipalityId;

        // Contacto de emergencia
        employee.EmergencyContactName = request.EmergencyContactName;
        employee.EmergencyContactPhone = request.EmergencyContactPhone;
        employee.EmergencyContactRelationship = request.EmergencyContactRelationship;

        // Datos fiscales
        employee.NORUC = request.NORUC;
        employee.NOINSS = request.NOINSS;

        // Asignación laboral
        employee.BranchId = request.BranchId;
        employee.PayrollGroupId = request.PayrollGroupId;
        employee.ContractTypeId = request.ContractTypeId;
        employee.BaseSalary = request.BaseSalary;

        // Condiciones especiales
        employee.IsTrustEmployee = request.IsTrustEmployee;
        employee.UsesTimeClock = request.UsesTimeClock;
        employee.BenefitsInKindValue = request.BenefitsInKindValue;
        employee.BenefitsInKindDescription = request.BenefitsInKindDescription;

        // Trabajador extranjero
        employee.Nationality = request.Nationality;
        employee.WorkPermitNumber = request.WorkPermitNumber;
        employee.WorkPermitExpirationDate = request.WorkPermitExpirationDate;

        // Datos bancarios
        employee.BankBeneficiaryName = request.BankBeneficiaryName;

        // Estado y notas
        employee.IsActive = request.IsActive;
        employee.Notes = request.Notes;

        // Imágenes
        employee.PhotoUrl = request.PhotoUrl;
        employee.IdFrontUrl = request.IdFrontUrl;
        employee.IdBackUrl = request.IdBackUrl;

        // Contratación
        employee.FirstHireDate = request.FirstHireDate;
    }

    /// <summary>
    /// Actualiza la identificación SOLO después de pasar validaciones de dominio.
    /// Separado porque requiere lógica externa (unicidad, formato cédula).
    /// </summary>
    public static void ApplyIdentificationChange(Employee employee, string validatedIdentification)
    {
        ArgumentNullException.ThrowIfNull(employee);

        var newCedula = (validatedIdentification ?? string.Empty).Trim().ToUpper();
        var currentCedula = (employee.Identification ?? string.Empty).Trim().ToUpper();

        if (!string.Equals(currentCedula, newCedula, StringComparison.Ordinal))
        {
            employee.Identification = newCedula;
        }
    }
}