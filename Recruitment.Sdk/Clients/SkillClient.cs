using System.Net.Http.Json;
using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Sdk.Clients
{
    public class SkillClient : ISkillClient
    {
        private readonly HttpClient _httpClient;

        public SkillClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SkillResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SkillResponse>>("api/skills", cancellationToken)
                   ?? Enumerable.Empty<SkillResponse>();
        }

        public async Task<SkillResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<SkillResponse>($"api/skills/{id}", cancellationToken);
        }

        public async Task<SkillResponse?> CreateAsync(CreateSkillRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/skills", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SkillResponse>(cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/skills/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
