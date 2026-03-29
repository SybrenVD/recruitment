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
            var prompt = $@"You are an expert HR recruiter analyzing a candidate's CV. Provide a comprehensive analysis in the following format:

## Summary
[2-3 sentences about the candidate's overall profile]

## Key Strengths
- [Bullet point 1]
- [Bullet point 2]
- [Bullet point 3]

## Areas for Development
- [Bullet point 1]
- [Bullet point 2]

## Recommended Skills to Learn
- [Skill 1]
- [Skill 2]
- [Skill 3]

## Experience Level Assessment
[Assessment based on CV content]

## Job Search Recommendations
[2-3 actionable recommendations]

---
CV Content:
{cvText}
---
Candidate: {candidateName}
Location: {location}";

            var requestBody = new
            {
                model = _isAzure ? _azureDeployment : _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = 1500,
                temperature = 0.7
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
        var sb = new System.Text.StringBuilder();
        
        sb.AppendLine("## Summary");
        sb.AppendLine($"{candidateName} is a candidate based in {location ?? "unspecified location"}.");
        
        if (!string.IsNullOrEmpty(cvText) && cvText.Length > 20)
        {
            var wordCount = cvText.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            sb.AppendLine($"A CV has been provided with approximately {wordCount} words of content.");
            
            var lines = cvText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 3)
                .Take(20)
                .ToList();
            
            if (lines.Any())
            {
                sb.AppendLine();
                sb.AppendLine("## Key Content Found in CV:");
                foreach (var line in lines.Take(8))
                {
                    sb.AppendLine($"• {line}");
                }
            }
            
            var keywords = new[] { "experience", "skills", "education", "certification", "project", "achievement", "responsibility" };
            var foundKeywords = keywords.Where(k => cvText.ToLower().Contains(k)).ToList();
            if (foundKeywords.Any())
            {
                sb.AppendLine();
                sb.AppendLine("## CV Sections Detected:");
                foreach (var kw in foundKeywords)
                {
                    sb.AppendLine($"• {char.ToUpper(kw[0]) + kw.Substring(1)} section found");
                }
            }
            
            var techKeywords = new[] { "c#", "java", "python", "javascript", "react", "angular", "node", "sql", "aws", "azure", "docker", "kubernetes", "git", "html", "css", "typescript", ".net", "spring", "django" };
            var foundTech = techKeywords.Where(t => cvText.ToLower().Contains(t)).ToList();
            if (foundTech.Any())
            {
                sb.AppendLine();
                sb.AppendLine("## Technical Skills Detected:");
                foreach (var tech in foundTech.Take(10))
                {
                    sb.AppendLine($"• {tech.ToUpper()}");
                }
            }
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine("## Key Strengths");
            sb.AppendLine("- CV documentation available for review");
            sb.AppendLine();
            sb.AppendLine("## Areas for Development");
            sb.AppendLine("- Upload a complete CV to enable detailed analysis");
        }
        
        sb.AppendLine();
        sb.AppendLine("## Recommended Skills to Learn");
        sb.AppendLine("- Communication skills");
        sb.AppendLine("- Problem-solving abilities");
        sb.AppendLine("- Technical expertise in your field");
        sb.AppendLine();
        sb.AppendLine("## Experience Level Assessment");
        if (!string.IsNullOrEmpty(cvText) && cvText.Length > 100)
        {
            sb.AppendLine("CV appears to have substantial content.");
        }
        else
        {
            sb.AppendLine("CV content appears limited - consider adding more details.");
        }
        sb.AppendLine();
        sb.AppendLine("## Job Search Recommendations");
        sb.AppendLine("- Ensure your CV is up to date");
        sb.AppendLine("- Highlight your key achievements");
        sb.AppendLine("- Customize your CV for each application");

        return sb.ToString();
    }
}
