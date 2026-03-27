using System.Net.Http.Json;
using Recruitment.Entities;

namespace Recruitement.Ui.Blazor.ApiClients;

public interface ICandidateApiClient
{
    Task<IEnumerable<Candidate>> GetAllAsync();
    Task<Candidate?> GetByIdAsync(int id);
    Task<Candidate> CreateAsync(Candidate candidate);
    Task UpdateAsync(Candidate candidate);
    Task DeleteAsync(int id);
    Task<Candidate?> GetByEmailAsync(string email);
    // Add more as needed
}

public class CandidateApiClient : ICandidateApiClient
{
    private readonly HttpClient _httpClient;

    public CandidateApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Candidate>> GetAllAsync()
        => await _httpClient.GetFromJsonAsync<IEnumerable<Candidate>>("") ?? new List<Candidate>();

    public async Task<Candidate?> GetByIdAsync(int id)
        => await _httpClient.GetFromJsonAsync<Candidate>($"{id}");

    public async Task<Candidate> CreateAsync(Candidate candidate)
    {
        var response = await _httpClient.PostAsJsonAsync("", candidate);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Candidate>())!;
    }

    public async Task UpdateAsync(Candidate candidate)
    {
        var response = await _httpClient.PutAsJsonAsync($"{candidate.Id}", candidate);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<Candidate?> GetByEmailAsync(string email)
        => await _httpClient.GetFromJsonAsync<Candidate>($"email/{email}");
}
