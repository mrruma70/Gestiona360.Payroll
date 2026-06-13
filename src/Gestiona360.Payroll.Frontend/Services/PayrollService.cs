using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Blazored.LocalStorage;

namespace Gestiona360.Payroll.Frontend.Services;

public class PayrollService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocalStorageService _localStorage;

    public PayrollService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
    {
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorage;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        // Usa la misma clave con la que guardas el token en JwtAuthStateProvider
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    public async Task<List<PayrollRecordResponse>> GetPayrollRecordsAsync(Guid periodId)
    {
        var client = await CreateAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<List<PayrollRecordResponse>>($"api/v1/payroll/periods/{periodId}/records") ?? new();
    }

    public async Task<PayrollCalculationResponse> CalculatePayrollAsync(Guid periodId)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync($"api/v1/payroll/periods/{periodId}/calculate", new { });
        return await response.Content.ReadFromJsonAsync<PayrollCalculationResponse>()
               ?? new PayrollCalculationResponse(0, 0, false);
    }

    public async Task<bool> ClosePeriodAsync(Guid periodId)
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync($"api/v1/payroll/periods/{periodId}/close", new { });
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