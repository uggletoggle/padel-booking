using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PadelBooking.Api.Services;

public class KeycloakAdminService(HttpClient httpClient, IConfiguration configuration)
{
    private readonly string _authority = configuration["Keycloak:Authority"] 
                                         ?? throw new ArgumentNullException("Keycloak:Authority");
    private readonly string _clientId = configuration["Keycloak:AdminClientId"] 
                                        ?? throw new ArgumentNullException("Keycloak:AdminClientId is required for Admin API");
    private readonly string _clientSecret = configuration["Keycloak:AdminClientSecret"] 
                                            ?? throw new ArgumentNullException("Keycloak:AdminClientSecret is required for Admin API");

    public async Task<string> CreateUserAsync(string email, string firstName, string lastName, string password)
    {
        var token = await GetAdminTokenAsync();
        
        var adminUrl = _authority.Replace("/realms/", "/admin/realms/") + "/users";

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, adminUrl);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var userPayload = new
        {
            username = email,
            email = email,
            firstName = firstName,
            lastName = lastName,
            enabled = true,
            emailVerified = true,
            credentials = new[]
            {
                new { type = "password", value = password, temporary = false }
            }
        };

        requestMessage.Content = new StringContent(JsonSerializer.Serialize(userPayload), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(requestMessage);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                throw new InvalidOperationException("User with this email already exists in Keycloak.");
            }

            throw new Exception($"Failed to create user in Keycloak. Status: {response.StatusCode}. Body: {errorBody}");
        }

        var location = response.Headers.Location?.ToString();
        if (string.IsNullOrEmpty(location))
        {
            throw new Exception("User created in Keycloak, but no Location header returned to extract ID.");
        }

        var userId = location.Split('/').Last();
        return userId;
    }

    public async Task DeleteUserAsync(string userId)
    {
        var token = await GetAdminTokenAsync();
        
        var adminUrl = _authority.Replace("/realms/", "/admin/realms/") + $"/users/{userId}";

        using var requestMessage = new HttpRequestMessage(HttpMethod.Delete, adminUrl);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.SendAsync(requestMessage);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"CRITICAL: Compensation transaction failed. Could not delete Keycloak user {userId}. Status: {response.StatusCode}. Body: {errorBody}");
        }
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_authority}/protocol/openid-connect/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _clientId },
            { "client_secret", _clientSecret }
        });

        var response = await httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get admin token. Status: {response.StatusCode}, Body: {errorBody}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<JsonElement>(json);
        
        return tokenResponse.GetProperty("access_token").GetString()!;
    }
}
