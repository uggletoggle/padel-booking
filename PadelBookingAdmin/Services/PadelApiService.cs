using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using PadelBookingAdmin.Models.Clubs;
using PadelBookingAdmin.Models.Users;

namespace PadelBookingAdmin.Services
{
    public class PadelApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PadelApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("PadelApi");
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task AuthenticateClientAsync()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    throw new Exception("Access token not found in current session. User may need to re-login.");
                }
            }
        }

        // Clubs
        public async Task<List<ClubResponse>> GetClubsAsync()
        {
            await AuthenticateClientAsync();
            var response = await _httpClient.GetAsync("/api/clubs");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ClubResponse>>() ?? new List<ClubResponse>();
        }

        public async Task<ClubResponse?> GetClubAsync(Guid id)
        {
            await AuthenticateClientAsync();
            var response = await _httpClient.GetAsync($"/api/clubs/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ClubResponse>();
            }
            return null;
        }

        public async Task<ClubResponse?> CreateClubAsync(CreateClubRequest request)
        {
            await AuthenticateClientAsync();
            var response = await _httpClient.PostAsJsonAsync("/api/clubs", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClubResponse>();
        }

        public async Task UpdateClubAsync(Guid id, UpdateClubRequest request)
        {
            await AuthenticateClientAsync();
            var response = await _httpClient.PutAsJsonAsync($"/api/clubs/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteClubAsync(Guid id)
        {
            await AuthenticateClientAsync();
            var response = await _httpClient.DeleteAsync($"/api/clubs/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Users
        public async Task<List<UserResponse>> GetUsersAsync()
        {
            await AuthenticateClientAsync();
            var response = await _httpClient.GetAsync("/api/users");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<UserResponse>>() ?? new List<UserResponse>();
        }

        public async Task<UserResponse?> CreateUserAsync(CreateAdminUserRequest request)
        {
            await AuthenticateClientAsync();
            var response = await _httpClient.PostAsJsonAsync("/api/users", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserResponse>();
        }
    }
}
