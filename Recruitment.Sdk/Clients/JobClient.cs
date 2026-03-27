using System.Net.Http.Json;
using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Sdk.Clients
{
    public class JobClient : IJobClient
    {
        private readonly HttpClient _httpClient;

        public JobClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<JobResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<JobResponse>>("api/jobs", cancellationToken)
                   ?? Enumerable.Empty<JobResponse>();
        }

        public async Task<JobResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<JobResponse>($"api/jobs/{id}", cancellationToken);
        }

        public async Task<JobResponse?> CreateAsync(CreateJobRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/jobs", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<JobResponse>(cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/jobs/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
