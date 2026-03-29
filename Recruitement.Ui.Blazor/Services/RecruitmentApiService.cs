using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Forms;
using Models = Recruitement.Ui.Blazor.Models;

namespace Recruitement.Ui.Blazor.Services;

public class RecruitmentApiService
{
    private readonly HttpClient _http;
    private readonly AuthService _auth;

    public RecruitmentApiService(HttpClient http, AuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    private async Task AddAuthHeader()
    {
        var token = await _auth.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<Models.Candidate>> GetCandidatesAsync()
    {
        await AddAuthHeader();
        var response = await _http.GetFromJsonAsync<List<Models.Candidate>>("api/candidates");
        return response ?? new List<Models.Candidate>();
    }

    public async Task<Models.Candidate?> GetCandidateAsync(int id)
    {
        return await _http.GetFromJsonAsync<Models.Candidate>($"api/candidates/{id}");
    }

    public async Task<Models.Candidate?> GetCandidateByEmailAsync(string email)
    {
        return await _http.GetFromJsonAsync<Models.Candidate>($"api/candidates/email/{email}");
    }

    public async Task<List<Models.Job>> GetJobsAsync(string? searchTerm = null, string? location = null)
    {
        await AddAuthHeader();
        var url = "api/jobs";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"searchTerm={searchTerm}");
        if (!string.IsNullOrEmpty(location)) queryParams.Add($"location={location}");
        if (queryParams.Any()) url += "?" + string.Join("&", queryParams);
        
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error {response.StatusCode}: {error}");
        }
        
        var json = await response.Content.ReadAsStringAsync();
        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };
        var jobs = System.Text.Json.JsonSerializer.Deserialize<List<Models.Job>>(json, options);
        return jobs ?? new List<Models.Job>();
    }

    public async Task<List<Models.Job>> GetAvailableJobsAsync(int candidateId)
    {
        await AddAuthHeader();
        var response = await _http.GetAsync($"api/jobs/available/{candidateId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error {response.StatusCode}: {error}");
        }
        
        var json = await response.Content.ReadAsStringAsync();
        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };
        var jobs = System.Text.Json.JsonSerializer.Deserialize<List<Models.Job>>(json, options);
        return jobs ?? new List<Models.Job>();
    }

    public async Task<Models.Job?> GetJobAsync(int id)
    {
        return await _http.GetFromJsonAsync<Models.Job>($"api/jobs/{id}");
    }

    public async Task<List<Models.JobMatch>> GetCandidateSuggestionsAsync(int jobId, int limit = 10)
    {
        var response = await _http.GetFromJsonAsync<List<Models.JobMatch>>($"api/jobs/{jobId}/candidates?limit={limit}");
        return response ?? new List<Models.JobMatch>();
    }

    public async Task<List<Models.JobMatch>> GetJobSuggestionsAsync(int candidateId, int limit = 10)
    {
        await AddAuthHeader();
        var response = await _http.GetAsync($"api/matching/candidates/{candidateId}/suggestions?limit={limit}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error: {error}");
        }
        var result = await response.Content.ReadFromJsonAsync<List<Models.JobMatch>>();
        return result ?? new List<Models.JobMatch>();
    }

    public async Task<bool> SwipeAsync(int candidateId, int jobId, bool isLike)
    {
        await AddAuthHeader();
        var response = await _http.PostAsJsonAsync("api/matching/swipe", new { candidateId, jobId, isLike });
        if (!response.IsSuccessStatusCode) return false;
        
        var json = await response.Content.ReadAsStringAsync();
        var doc = System.Text.Json.JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("isMatch").GetBoolean();
    }

    public async Task<List<Models.JobMatch>> GetMutualMatchesAsync(int candidateId)
    {
        await AddAuthHeader();
        var response = await _http.GetAsync($"api/matching/mutual?userId={candidateId}&isCandidate=true");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error: {error}");
        }
        var result = await response.Content.ReadFromJsonAsync<List<Models.JobMatch>>();
        return result ?? new List<Models.JobMatch>();
    }

    public async Task<List<Models.JobMatch>> GetRecruiterMutualMatchesAsync(int recruiterId)
    {
        await AddAuthHeader();
        var response = await _http.GetAsync($"api/matching/mutual?userId={recruiterId}&isCandidate=false");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error: {error}");
        }
        var result = await response.Content.ReadFromJsonAsync<List<Models.JobMatch>>();
        return result ?? new List<Models.JobMatch>();
    }

    public async Task<List<Models.JobMatch>> GetAvailableCandidatesForRecruiterAsync(int recruiterId, int jobId)
    {
        await AddAuthHeader();
        var response = await _http.GetAsync($"api/matching/recruiter/{recruiterId}/job/{jobId}/available");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error: {error}");
        }
        var result = await response.Content.ReadFromJsonAsync<List<Models.JobMatch>>();
        return result ?? new List<Models.JobMatch>();
    }

    public async Task<Models.CVAnalysis?> GetAnalysisAsync(int candidateId)
    {
        return await _http.GetFromJsonAsync<Models.CVAnalysis>($"api/candidates/{candidateId}/analysis");
    }

    public async Task<Models.CVAnalysis> AnalyzeCVAsync(int candidateId)
    {
        return await _http.GetFromJsonAsync<Models.CVAnalysis>($"api/candidates/{candidateId}/analyze") 
            ?? throw new Exception("Analysis failed");
    }

    public async Task<List<Models.InterviewQuestion>> GetInterviewQuestionsAsync(int candidateId, int jobId)
    {
        var response = await _http.GetFromJsonAsync<List<Models.InterviewQuestion>>($"api/candidates/{candidateId}/questions/{jobId}");
        return response ?? new List<Models.InterviewQuestion>();
    }

    public async Task<List<Models.InterviewQuestion>> GenerateInterviewQuestionsAsync(int candidateId, int jobId)
    {
        var response = await _http.PostAsJsonAsync($"api/candidates/{candidateId}/questions/{jobId}", new { });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Models.InterviewQuestion>>() ?? new List<Models.InterviewQuestion>();
    }

    public async Task<List<Models.Skill>> GetSkillsAsync()
    {
        var response = await _http.GetFromJsonAsync<List<Models.Skill>>("api/skills");
        return response ?? new List<Models.Skill>();
    }

    public async Task<Models.Job> CreateJobAsync(Models.Job job, List<object>? skills = null)
    {
        var request = new { job, skills };
        var response = await _http.PostAsJsonAsync("api/jobs", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Models.Job>() ?? throw new Exception("Failed to create job");
    }

    public async Task AddJobSkillAsync(int jobId, int skillId, int requiredLevel, int weight = 1)
    {
        await _http.PostAsJsonAsync($"api/jobs/{jobId}/skills", new { skillId, requiredLevel, weight });
    }

    public async Task<Models.Job> CreateJobWithSkillsAsync(Models.Job job, List<(string name, int level)> skills)
    {
        await AddAuthHeader();
        var request = new
        {
            title = job.Title,
            description = job.Description,
            location = job.Location,
            experienceLevel = job.ExperienceLevel,
            recruiterId = job.RecruiterId,
            skills = skills.Select(s => new { name = s.name, level = s.level }).ToList()
        };
        var response = await _http.PostAsJsonAsync("api/jobs/create-with-skills", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Models.Job>() ?? throw new Exception("Failed to create job");
    }

    public async Task<List<Models.Candidate>> GetCandidatesForJobAsync(int jobId, int limit = 10)
    {
        var response = await _http.GetAsync($"api/jobs/{jobId}/candidates?limit={limit}");
        if (!response.IsSuccessStatusCode)
        {
            return new List<Models.Candidate>();
        }
        var result = await response.Content.ReadFromJsonAsync<List<Models.Candidate>>();
        return result ?? new List<Models.Candidate>();
    }

    public async Task<List<Models.JobMatch>> GetJobSuggestionsForRecruiterAsync(int jobId, int limit = 10)
    {
        var response = await _http.GetAsync($"api/matching/jobs/{jobId}/suggestions?limit={limit}");
        if (!response.IsSuccessStatusCode)
        {
            return new List<Models.JobMatch>();
        }
        var result = await response.Content.ReadFromJsonAsync<List<Models.JobMatch>>();
        return result ?? new List<Models.JobMatch>();
    }

    public async Task<bool> SwipeCandidateAsync(int candidateId, int jobId, bool isLike)
    {
        await AddAuthHeader();
        var response = await _http.PostAsJsonAsync("api/matching/recruiter-swipe", new { candidateId, jobId, isLike });
        if (!response.IsSuccessStatusCode) return false;
        
        var json = await response.Content.ReadAsStringAsync();
        var doc = System.Text.Json.JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("isMatch").GetBoolean();
    }

    public async Task<bool> UploadCVAsync(int candidateId, IBrowserFile file)
    {
        using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var response = await _http.PostAsync($"api/candidates/{candidateId}/cv", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RegisterAsync(string firstName, string lastName, string email, string password, string location, string userType, string? company)
    {
        var request = new
        {
            firstName,
            lastName,
            email,
            password,
            location,
            userType,
            company
        };
        var response = await _http.PostAsJsonAsync("api/auth/register", request);
        return response.IsSuccessStatusCode;
    }

}
