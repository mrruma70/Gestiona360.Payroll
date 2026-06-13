using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

namespace Gestiona360.Payroll.Frontend.Auth
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private const string TOKEN_KEY = "authToken";

        public JwtAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = null;

            try
            {
                token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
            }
            catch
            {
                // Si falla, devolvemos no autenticado
            }

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            try
            {
                var claims = ParseClaims(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            catch
            {
                await _localStorage.RemoveItemAsync(TOKEN_KEY);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task<string> GetTokenAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
                return token ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task SetTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                await _localStorage.RemoveItemAsync(TOKEN_KEY);
            }
            else
            {
                await _localStorage.SetItemAsync(TOKEN_KEY, token);
                Console.WriteLine($"✅ Token guardado en localStorage ({token.Length} chars)");
            }

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotifyUserLoggedIn()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(TOKEN_KEY);
            NotifyAuthenticationStateChanged(Task.FromResult(
                new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            );
        }

        private IEnumerable<Claim> ParseClaims(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
                return new List<Claim>();

            var parts = jwt.Split('.');
            if (parts.Length != 3)
                return new List<Claim>();

            var payload = parts[1];
            byte[] jsonBytes;

            try
            {
                jsonBytes = ParseBase64WithoutPadding(payload);
            }
            catch
            {
                return new List<Claim>();
            }

            Dictionary<string, object> keyValuePairs;
            try
            {
                keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            }
            catch
            {
                return new List<Claim>();
            }

            if (keyValuePairs == null)
                return new List<Claim>();

            return keyValuePairs.Select(kvp =>
            {
                string claimValue;
                if (kvp.Value is JsonElement element)
                {
                    switch (element.ValueKind)
                    {
                        case JsonValueKind.String:
                            claimValue = element.GetString();
                            break;
                        case JsonValueKind.Number:
                            claimValue = element.GetRawText();
                            break;
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            claimValue = element.GetBoolean().ToString();
                            break;
                        case JsonValueKind.Array:
                            claimValue = string.Join(",", element.EnumerateArray().Select(x => x.ToString()));
                            break;
                        default:
                            claimValue = element.GetRawText();
                            break;
                    }
                }
                else
                {
                    claimValue = kvp.Value?.ToString() ?? "";
                }
                return new Claim(kvp.Key, claimValue);
            });
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}