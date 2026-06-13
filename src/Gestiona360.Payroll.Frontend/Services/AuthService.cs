using System.Net.Http.Json;

namespace Gestiona360.Payroll.Frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<LoginResponse> Login(string email, string password)
        {
            try
            {
                var request = new
                {
                    email = email,
                    password = password,
                    tenantCode = "GESTIONA360"
                };

                var response = await _http.PostAsJsonAsync("api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.AccessToken))
                    {
                        result.Success = true;
                        return result;
                    }
                    return new LoginResponse { Success = false, Message = "Respuesta sin token" };
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return new LoginResponse
                    {
                        Success = false,
                        Message = $"Error {response.StatusCode}: {error}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}"
                };
            }
        }
    }


    // ✅ Debe coincidir EXACTAMENTE con lo que devuelve tu API
    // ✅ Debe coincidir EXACTAMENTE con lo que devuelve tu API
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public UserInfo User { get; set; } = new();

        // ✅ AHORA TIENEN SETTERS
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        // ✅ Propiedad auxiliar para obtener el token
        public string Token => AccessToken;
    }

    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}