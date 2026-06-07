using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Gestiona360.Payroll.Frontend.Services;

public class PayrollService
{
    private readonly HttpClient _httpClient;

    public PayrollService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PayrollRecordResponse>> GetPayrollRecordsAsync(Guid periodId)
    {
        return await _httpClient.GetFromJsonAsync<List<PayrollRecordResponse>>($"api/v1/payroll/periods/{periodId}/records") ?? new();
    }

    public async Task<PayrollCalculationResponse> CalculatePayrollAsync(Guid periodId)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/v1/payroll/periods/{periodId}/calculate", new { });

        // CORRECCIÓN: Se pasan argumentos explícitos al inicializar el record si la respuesta es nula
        return await response.Content.ReadFromJsonAsync<PayrollCalculationResponse>()
               ?? new PayrollCalculationResponse(0, 0, false);
    }

    public async Task<bool> ClosePeriodAsync(Guid periodId)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/v1/payroll/periods/{periodId}/close", new { });
        return response.IsSuccessStatusCode;
    }
}

public record PayrollRecordResponse(
    Guid Id,
    string EmployeeName,
    decimal GrossIncome,
    decimal INSSWorker,
    decimal IR,
    decimal NetPay,
    string Status
);

public record PayrollCalculationResponse(
    int TotalRecords,
    decimal TotalNetPay,
    bool Success
);
