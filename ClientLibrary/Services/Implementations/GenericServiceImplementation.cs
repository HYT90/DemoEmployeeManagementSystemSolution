
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using System.Diagnostics;
using System.Net.Http.Json;

namespace ClientLibrary.Services.Implementations
{
    public class GenericServiceImplementation<T>(GetHttpClient getHttpClient) : IGenericServiceInterface<T>
    {
        // Create
        public async Task<GeneralResponse> Insert(T item, string baseUrl)
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var response = await httpClient.PostAsJsonAsync($"{baseUrl}/add", item);
            var res = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return res!;
        }

        // Read All
        public async Task<List<T>> GetAll(string baseUrl)
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var res = await httpClient.GetFromJsonAsync<List<T>>($"{baseUrl}/all");
            return res!;
        }

        // Read Single {id}
        public async Task<T> GetById(int id, string baseUrl)
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var res = await httpClient.GetFromJsonAsync<T>($"{baseUrl}/single/{id}");
            return res!;
        }

        // Update {model}
        public async Task<GeneralResponse> Update(T item, string baseUrl)
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var response = await httpClient.PutAsJsonAsync($"{baseUrl}/update", item);
            var res = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return res!;
        }

        // Delete {id}
        public async Task<GeneralResponse> DeleteById(int id, string baseUrl)
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var response = await httpClient.DeleteAsync($"{baseUrl}/delete/{id}");
            var res = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return res!;
        }
    }
}
