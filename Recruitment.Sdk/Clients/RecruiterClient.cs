using System.Net.Http.Json;
using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Sdk.Clients
{
    public class RecruiterClient : IRecruiterClient
    {
        private readonly HttpClient _httpClient;

        public RecruiterClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<RecruiterResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RecruiterResponse>>("api/recruiters", cancellationToken)
                   ?? Enumerable.Empty<RecruiterResponse>();
        }

        public async Task<RecruiterResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<RecruiterResponse>($"api/recruiters/{id}", cancellationToken);
        }

        public async Task<RecruiterResponse?> CreateAsync(CreateRecruiterRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/recruiters", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RecruiterResponse>(cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/recruiters/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
