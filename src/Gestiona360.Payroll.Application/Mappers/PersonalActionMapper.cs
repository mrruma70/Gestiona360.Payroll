// src/Gestiona360.Payroll.Application/Mappers/PersonalActionMapper.cs

using System.Text.Json;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;

// ✅ CORREGIR: Namespace correcto
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Application.Mappers;

public static class PersonalActionMapper
{
    public static PersonalActionDto ToDto(PersonalActionWithDetails pa)
    {
        return new PersonalActionDto
        {
            Id = pa.Id,
            EffectiveDate = pa.EffectiveDate,
            // ✅ CORREGIR: ActionType en lugar de PersonalActionType
            ActionType = pa.ActionType.ToString(),
            // ✅ CORREGIR: ActionStatus en lugar de PersonalActionStatus
            Status = pa.Status.ToString(),
            Justification = pa.Justification ?? string.Empty,
            CausalDescription = pa.CausalDescription,
            ExecutedDate = pa.ExecutedDate,
            ApprovedBy = pa.ApprovedBy ?? Guid.Empty,
            ApprovedByName = pa.ApprovedByName ?? "Sistema",

            // Salario
            OldBaseSalary = pa.OldBaseSalary,
            NewBaseSalary = pa.NewBaseSalary,

            // Puesto
            OldJobGradeId = pa.OldJobGradeId,
            OldJobGradeName = pa.OldJobGradeName,
            NewJobGradeId = pa.NewJobGradeId,
            NewJobGradeName = pa.NewJobGradeName,

            // Contrato
            OldContractTypeId = pa.OldContractTypeId,
            OldContractTypeName = pa.OldContractTypeName,
            NewContractTypeId = pa.NewContractTypeId,
            NewContractTypeName = pa.NewContractTypeName,

            // Turno
            OldShiftId = pa.OldShiftId,
            OldShiftName = pa.OldShiftName,
            NewShiftId = pa.NewShiftId,
            NewShiftName = pa.NewShiftName,

            // Centro de Costo
            OldCostCenterId = pa.OldCostCenterId,
            OldCostCenterName = pa.OldCostCenterName,
            NewCostCenterId = pa.NewCostCenterId,
            NewCostCenterName = pa.NewCostCenterName,

            // Compatibilidad con código antiguo
            OldValue = pa.OldBaseSalary.HasValue ? $"C$ {pa.OldBaseSalary.Value:N2}" : "N/A",
            NewValue = pa.NewBaseSalary.HasValue ? $"C$ {pa.NewBaseSalary.Value:N2}" : "N/A"
        };
    }

    /// <summary>
    /// Parsea documentos adjuntos desde JSON a lista de DTOs.
    /// Soporta múltiples formatos de JSON.
    /// </summary>
    public static List<DocumentAttachmentDto> ParseDocumentAttachments(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<DocumentAttachmentDto>();
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var trimmedJson = json.TrimStart();

            // FORMATO 1: Array directo
            if (trimmedJson.StartsWith("["))
            {
                return JsonSerializer.Deserialize<List<DocumentAttachmentDto>>(json, options)
                       ?? new List<DocumentAttachmentDto>();
            }

            // FORMATO 2: Objeto con propiedad "documents"
            if (trimmedJson.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("documents", out var documentsElement))
                {
                    var documentsJson = documentsElement.GetRawText();
                    return JsonSerializer.Deserialize<List<DocumentAttachmentDto>>(documentsJson, options)
                           ?? new List<DocumentAttachmentDto>();
                }

                if (doc.RootElement.TryGetProperty("DocumentAttachments", out var attachmentsElement))
                {
                    var attachmentsJson = attachmentsElement.GetRawText();
                    return JsonSerializer.Deserialize<List<DocumentAttachmentDto>>(attachmentsJson, options)
                           ?? new List<DocumentAttachmentDto>();
                }
            }

            return new List<DocumentAttachmentDto>();
        }
        catch
        {
            return new List<DocumentAttachmentDto>();
        }
    }
}
