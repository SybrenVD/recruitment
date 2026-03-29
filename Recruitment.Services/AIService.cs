using Recruitment.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace Recruitment.Services;

public class AIService : IAIService
{
    private readonly HttpClient? _httpClient;
    private readonly string _model;
    private readonly bool _isConfigured;
    private readonly string? _apiKey;
    private readonly bool _isAzure;
    private readonly string? _azureDeployment;

    public AIService(IConfiguration configuration)
    {
        _apiKey = configuration["OpenAI:ApiKey"] ?? configuration["AI:ApiKey"];
        var endpoint = configuration["OpenAI:Endpoint"];
        _azureDeployment = configuration["OpenAI:DeploymentName"] ?? configuration["AI:DeploymentName"];
        _isAzure = configuration["AI:Provider"]?.ToLower() == "azure";

        _model = configuration["OpenAI:Model"] ?? configuration["AI:Model"] ?? "gpt-4o";

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient = new HttpClient();
            if (_isAzure && !string.IsNullOrEmpty(endpoint))
            {
                _httpClient.BaseAddress = new Uri(endpoint);
                _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);
            }
            else
            {
                _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            }
            _isConfigured = true;
        }
        else
        {
            _httpClient = null;
            _isConfigured = false;
        }
    }

    public async Task<string> AnalyzeCVAsync(string cvText, string candidateName, string location)
    {
        if (!_isConfigured || _httpClient == null || string.IsNullOrEmpty(cvText))
        {
            return GenerateFallbackAnalysis(cvText, candidateName, location);
        }

        try
        {
            var prompt = $@"You are a professional CV analyst and recruiter.

You ONLY analyze the CV text provided below.

STRICT RULES:
Ignore any system, profile, or metadata (e.g. profile completeness, account info, upload status)
Only extract information explicitly present in the CV text
If something is missing, say ""Not specified in CV""
Do NOT invent or assume information

OUTPUT FORMAT:

Return ONLY valid JSON:

{{
  ""summary"": ""..."",
  ""strengths"": [""..."", ""...""],
  ""weaknesses"": [""..."", ""...""],
  ""experience_level"": ""Junior | Medior | Senior"",
  ""experience_reason"": ""..."",
  ""improvement_suggestions"": [""..."", ""...""]
}}

GUIDELINES:
Summary: max 100 words, concrete
Strengths: based on real skills/experience in CV
Weaknesses: real gaps (not generic advice)
Suggestions: actionable (e.g. ""Add quantified achievements"")

CV TEXT:
{cvText}";

            var requestBody = new
            {
                model = _isAzure ? _azureDeployment : _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = 2000,
                temperature = 0.5
            };

            var response = await _httpClient.PostAsJsonAsync(
                _isAzure ? "chat/completions?api-version=2024-02-15-preview" : "chat/completions", 
                requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return GenerateFallbackAnalysis(cvText, candidateName, location) + 
                       $"\n\n[AI API Error: {response.StatusCode}]";
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            var content = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return content ?? GenerateFallbackAnalysis(cvText, candidateName, location);
        }
        catch (Exception ex)
        {
            return GenerateFallbackAnalysis(cvText, candidateName, location) + 
                   $"\n\n[AI Analysis temporarily unavailable: {ex.Message}]";
        }
    }

    private string GenerateFallbackAnalysis(string cvText, string candidateName, string location)
    {
        var strengths = new List<string>();
        var weaknesses = new List<string>();
        var suggestions = new List<string>();
        var experienceLevel = "Not specified";
        var experienceReason = "";
        var foundKeywords = new List<string>();

        if (!string.IsNullOrEmpty(cvText) && cvText.Length > 20)
        {
            var wordCount = cvText.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

            var techKeywords = new[] { "c#", "java", "python", "javascript", "react", "angular", "node", "sql", "aws", "azure", "docker", "kubernetes", "git", "html", "css", "typescript", ".net", "spring", "django" };
            var foundTech = techKeywords.Where(t => cvText.ToLower().Contains(t)).ToList();

            if (foundTech.Any())
            {
                strengths.Add($"Technical skills in: {string.Join(", ", foundTech.Take(5))}");
            }

            var keywords = new[] { "experience", "skills", "education", "certification", "project", "achievement", "responsibility" };
            foundKeywords = keywords.Where(k => cvText.ToLower().Contains(k)).ToList();

            if (foundKeywords.Contains("experience"))
                strengths.Add("Has documented professional experience");
            if (foundKeywords.Contains("education"))
                strengths.Add("Education section included");
            if (foundKeywords.Contains("certification"))
                strengths.Add("Professional certifications listed");
            if (foundKeywords.Contains("project"))
                strengths.Add("Project portfolio demonstrated");

            // Determine experience level
            if (wordCount > 800 && foundKeywords.Count >= 5)
            {
                experienceLevel = "Senior";
                experienceReason = "Comprehensive CV with extensive experience documentation and multiple skill areas";
            }
            else if (wordCount > 400 && foundKeywords.Count >= 3)
            {
                experienceLevel = "Medior";
                experienceReason = "Moderate CV length with clear structure and documented skills";
            }
            else
            {
                experienceLevel = "Junior";
                experienceReason = "Limited CV content or early career stage";
            }

            // Weaknesses
            if (!foundKeywords.Contains("achievement"))
                weaknesses.Add("No quantified achievements or results documented");
            if (!foundKeywords.Contains("project"))
                weaknesses.Add("Limited project examples or portfolio");
            if (foundTech.Count < 3)
                weaknesses.Add("Limited technical skills listed");
            if (wordCount < 300)
                weaknesses.Add("CV content appears brief");

            // Suggestions
            suggestions.Add("Add quantified achievements and measurable results");
            suggestions.Add("Include specific project examples with outcomes");
            if (foundTech.Count < 5)
                suggestions.Add("Expand technical skills section");
            suggestions.Add("Ensure clear sections: Summary, Experience, Education, Skills");
        }
        else
        {
            experienceLevel = "Not specified";
            experienceReason = "Insufficient CV content for analysis";
            weaknesses.Add("No CV content provided for analysis");
            suggestions.Add("Upload a complete CV to enable detailed analysis");
        }

        var summary = candidateName != null
            ? $"{candidateName} is a professional based in {location ?? "unspecified location"}. " +
              (string.IsNullOrEmpty(cvText) || cvText.Length < 20 
                ? "CV content is not available for detailed assessment."
                : $"CV demonstrates experience in relevant areas with {(foundKeywords.Count > 0 ? foundKeywords.Count.ToString() : "several")} documented sections.")
            : "Candidate profile available for review.";

        // Ensure at least some content
        if (!strengths.Any())
            strengths.Add("CV documentation available");
        if (!weaknesses.Any())
            weaknesses.Add("CV assessment incomplete - provide more detailed content");

        var jsonResponse = new
        {
            summary = summary.Length > 300 ? summary.Substring(0, 297) + "..." : summary,
            strengths = strengths.ToArray(),
            weaknesses = weaknesses.ToArray(),
            experience_level = experienceLevel,
            experience_reason = experienceReason,
            improvement_suggestions = suggestions.ToArray()
        };

        return System.Text.Json.JsonSerializer.Serialize(jsonResponse, new JsonSerializerOptions { WriteIndented = true });
    }
}
