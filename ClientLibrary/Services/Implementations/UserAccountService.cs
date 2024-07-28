
using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using System.Net.Http.Json;

namespace ClientLibrary.Services.Implementations
{
    public class UserAccountService(GetHttpClient getHttpClient) : IUserAccountService
    {
        public const string AuthUrl = "api/authentication";
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var res = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
            if (!res.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");

            return await res.Content.ReadFromJsonAsync<GeneralResponse>()!;
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var res = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);
            if (!res.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");

            return await res.Content.ReadFromJsonAsync<LoginResponse>()!;
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var res = await httpClient.PostAsJsonAsync($"{AuthUrl}/refresh-token", token);
            if (!res.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");

            return await res.Content.ReadFromJsonAsync<LoginResponse>()!;
        }

        public async Task<List<ManageUser>> GetUsers()
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var res = await httpClient.GetFromJsonAsync<List<ManageUser>>($"{AuthUrl}/users");
            return res!;
        }

        public async Task<GeneralResponse> UpdateUser(ManageUser user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var res = await httpClient.PutAsJsonAsync($"{AuthUrl}/update-user", user);
            if (!res.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");
            return await res.Content.ReadFromJsonAsync<GeneralResponse>()!;
        }

        public async Task<List<SystemRole>> GetRoles()
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var res = await httpClient.GetFromJsonAsync<List<SystemRole>>($"{AuthUrl}/roles");
            return res!;
        }

        public async Task<GeneralResponse> DeleteUser(int id)
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var res = await httpClient.DeleteAsync($"{AuthUrl}/delete-user/{id}");
            if (!res.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");
            return await res.Content.ReadFromJsonAsync<GeneralResponse>()!;
        }
    }
}
