using System.Net.Http.Json;
using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Sdk.Clients
{
    public class CandidateClient : ICandidateClient
    {
        private readonly HttpClient _httpClient;

        public CandidateClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CandidateResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CandidateResponse>>("api/candidates", cancellationToken)
                   ?? Enumerable.Empty<CandidateResponse>();
        }

        public async Task<CandidateResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<CandidateResponse>($"api/candidates/{id}", cancellationToken);
        }

        public async Task<CandidateResponse?> CreateAsync(CreateCandidateRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/candidates", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CandidateResponse>(cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/candidates/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
