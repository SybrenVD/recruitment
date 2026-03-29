using Recruitment.Interfaces;
using System.Text;
using System.Text.Json;

namespace Recruitment.Services;

public class CVAnalysisService : ICVAnalysisService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPdfReaderService _pdfReader;
    private readonly IAIService _aiService;
    private readonly string _uploadBasePath;

    public CVAnalysisService(
        IUnitOfWork unitOfWork, 
        IPdfReaderService pdfReader,
        IAIService aiService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _pdfReader = pdfReader;
        _aiService = aiService;
        _uploadBasePath = configuration.GetValue<string>("FileStorage:BasePath") ?? "wwwroot/uploads";
    }

    public async Task<Entities.CVAnalysis> AnalyzeCVAsync(int candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(candidateId);
        if (candidate == null)
            throw new InvalidOperationException("Candidate not found");

        var existingAnalysis = (await _unitOfWork.CVAnalyses.FindAsync(a => a.CandidateId == candidateId)).FirstOrDefault();
        if (existingAnalysis != null)
        {
            await _unitOfWork.CVAnalyses.DeleteAsync(existingAnalysis.Id);
        }

        var candidateSkills = (await _unitOfWork.CandidateSkills.FindAsync(cs => cs.CandidateId == candidateId)).ToList();
        
        var cvText = string.Empty;
        var cvReadError = false;
        if (!string.IsNullOrEmpty(candidate.CVFilePath))
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), _uploadBasePath, candidate.CVFilePath);
            cvText = await _pdfReader.ExtractTextAsync(fullPath);
            if (string.IsNullOrWhiteSpace(cvText))
            {
                cvReadError = true;
            }
        }

        var aiAnalysis = await _aiService.AnalyzeCVAsync(
            cvText, 
            $"{candidate.FirstName} {candidate.LastName}", 
            candidate.Location ?? "unspecified");

        if (cvReadError)
        {
            aiAnalysis = "{ \"error\": \"Your CV appears to be an image-based PDF (scanned or created from Google Docs/image). Text extraction cannot read image-based PDFs. Please upload a text-based PDF with actual text content.\" }" + aiAnalysis;
        }

        var (summary, strengths, weaknesses, experienceLevel) = ParseAIAnalysis(candidate, candidateSkills, cvText, aiAnalysis);

        var analysis = new Entities.CVAnalysis
        {
            CandidateId = candidateId,
            Summary = summary,
            ExperienceLevel = experienceLevel,
            Strengths = strengths,
            Weaknesses = weaknesses,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.CVAnalyses.AddAsync(analysis);

        return analysis;
    }

    private (string summary, string strengths, string weaknesses, string experienceLevel) ParseAIAnalysis(
        Entities.Candidate candidate, 
        List<Entities.CandidateSkill> skills, 
        string cvText, 
        string aiAnalysis)
    {
        var summary = GenerateSummary(candidate, skills, cvText);
        var strengths = GenerateStrengths(candidate, skills);
        var weaknesses = GenerateWeaknesses(candidate, skills);
        var experienceLevel = "Not specified";

        try
        {
            // Try to extract JSON from the response
            var jsonStart = aiAnalysis.IndexOf('{');
            var jsonEnd = aiAnalysis.LastIndexOf('}');

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonStr = aiAnalysis.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var jsonDoc = JsonDocument.Parse(jsonStr);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("summary", out var summaryElement))
                {
                    summary = (summaryElement.GetString() ?? summary).Trim();
                }

                if (root.TryGetProperty("strengths", out var strengthsArray))
                {
                    var strengthsList = new StringBuilder();
                    strengthsList.AppendLine("AI Analysis - Key Strengths:");
                    foreach (var strength in strengthsArray.EnumerateArray())
                    {
                        strengthsList.AppendLine($"• {strength.GetString()}");
                    }
                    strengths = strengthsList.ToString();
                }

                if (root.TryGetProperty("weaknesses", out var weaknessesArray))
                {
                    var weaknessesList = new StringBuilder();
                    weaknessesList.AppendLine("AI Analysis - Areas for Development:");
                    foreach (var weakness in weaknessesArray.EnumerateArray())
                    {
                        weaknessesList.AppendLine($"• {weakness.GetString()}");
                    }
                    if (root.TryGetProperty("improvement_suggestions", out var suggestionsArray))
                    {
                        weaknessesList.AppendLine();
                        weaknessesList.AppendLine("Improvement Suggestions:");
                        foreach (var suggestion in suggestionsArray.EnumerateArray())
                        {
                            weaknessesList.AppendLine($"• {suggestion.GetString()}");
                        }
                    }
                    weaknesses = weaknessesList.ToString();
                }

                if (root.TryGetProperty("experience_level", out var levelElement))
                {
                    var level = levelElement.GetString()?.Trim();
                    if (!string.IsNullOrEmpty(level))
                    {
                        experienceLevel = level;
                    }
                }
            }
        }
        catch
        {
            // If JSON parsing fails, use fallback analysis
        }

        return (summary, strengths, weaknesses, experienceLevel);
    }

    private string GenerateSummary(Entities.Candidate candidate, List<Entities.CandidateSkill> skills, string cvText)
    {
        var skillNames = skills.OrderByDescending(s => s.Level).Take(5).Select(s => s.Skill?.SkillName ?? "Unknown").ToList();
        var skillText = skillNames.Any() ? string.Join(", ", skillNames) : "various technical skills";

        var sb = new StringBuilder();
        sb.AppendLine($"Profile Analysis for {candidate.FirstName} {candidate.LastName}");
        sb.AppendLine();
        sb.AppendLine($"{candidate.FirstName} is a professional based in {candidate.Location ?? "unspecified location"} with expertise in {skillText}.");
        sb.AppendLine();

        if (skills.Any())
        {
            var avgLevel = skills.Average(s => s.Level);
            if (avgLevel >= 4)
            {
                sb.AppendLine("This candidate demonstrates advanced skills and would be suitable for senior or specialized positions.");
            }
            else if (avgLevel >= 3)
            {
                sb.AppendLine("This candidate has solid intermediate skills, suitable for mid-level positions with room for growth.");
            }
            else
            {
                sb.AppendLine("This candidate is developing their skill set and would be best suited for entry-level or junior positions.");
            }
        }

        if (!string.IsNullOrEmpty(cvText) && cvText.Length > 50)
        {
            sb.AppendLine("A CV has been uploaded with approximately " + (cvText.Split(' ').Length) + " words of content.");

            var keywords = new[] { "experience", "projects", "education", "certifications", "achievements" };
            var foundKeywords = keywords.Where(k => cvText.ToLower().Contains(k)).ToList();
            if (foundKeywords.Any())
            {
                sb.AppendLine("CV contains sections for: " + string.Join(", ", foundKeywords.Select(k => k.ToUpper())));
            }
        }
        else if (!string.IsNullOrEmpty(candidate.CVFilePath))
        {
            sb.AppendLine("A CV has been uploaded and is available for review.");
        }

        return sb.ToString();
    }

    private string GenerateStrengths(Entities.Candidate candidate, List<Entities.CandidateSkill> skills)
    {
        var sb = new StringBuilder();

        if (skills.Any())
        {
            var topSkills = skills.OrderByDescending(s => s.Level).Take(4).ToList();
            sb.AppendLine("Technical Strengths:");
            foreach (var skill in topSkills)
            {
                var skillName = skill.Skill?.SkillName ?? "Unknown Skill";
                var levelText = skill.Level switch
                {
                    5 => "Expert",
                    4 => "Advanced",
                    3 => "Proficient",
                    2 => "Intermediate",
                    _ => "Beginner"
                };
                sb.AppendLine($"• {skillName}: {levelText} (Level {skill.Level}/5)");
            }
        }

        sb.AppendLine();
        sb.AppendLine("Key Attributes:");
        sb.AppendLine("• Based in " + (candidate.Location ?? "unspecified location"));
        if (!string.IsNullOrEmpty(candidate.CVFilePath))
        {
            sb.AppendLine("• CV documentation available");
        }
        sb.AppendLine("• Profile created: " + candidate.CreatedAt.ToString("MMMM yyyy"));

        return sb.ToString();
    }

    private string GenerateWeaknesses(Entities.Candidate candidate, List<Entities.CandidateSkill> skills)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Areas for Development:");

        if (!skills.Any())
        {
            sb.AppendLine("• No skills currently listed - recommend adding technical skills to improve visibility");
        }
        else
        {
            var lowerSkills = skills.Where(s => s.Level < 3).ToList();
            if (lowerSkills.Any())
            {
                sb.AppendLine("• Skills that could use improvement:");
                foreach (var skill in lowerSkills.Take(3))
                {
                    var skillName = skill.Skill?.SkillName ?? "Unknown Skill";
                    sb.AppendLine($"  - {skillName} (Level {skill.Level}/5)");
                }
            }

            var skillGapAreas = GetSuggestedSkills(skills);
            if (skillGapAreas.Any())
            {
                sb.AppendLine();
                sb.AppendLine("• Recommended skills to learn:");
                foreach (var skill in skillGapAreas)
                {
                    sb.AppendLine($"  - {skill}");
                }
            }
        }

        if (string.IsNullOrEmpty(candidate.CVFilePath))
        {
            sb.AppendLine();
            sb.AppendLine("• No CV uploaded - recommend uploading a CV to increase chances of matching with employers");
        }

        return sb.ToString();
    }

    private List<string> GetSuggestedSkills(List<Entities.CandidateSkill> existingSkills)
    {
        var commonSkills = new List<(string name, string category)>
        {
            ("Git", "Tools"),
            ("Docker", "DevOps"),
            ("REST APIs", "Web"),
            ("Agile/Scrum", "Methodology"),
            ("Problem Solving", "Soft Skills"),
            ("Communication", "Soft Skills"),
            ("AWS", "Cloud"),
            ("SQL", "Database"),
            ("React", "Frontend"),
            (".NET", "Backend")
        };

        var existingNames = existingSkills.Select(s => (s.Skill?.SkillName ?? "").ToLower()).ToHashSet();
        return commonSkills
            .Where(s => !existingNames.Contains(s.name.ToLower()))
            .Take(3)
            .Select(s => s.name)
            .ToList();
    }

    private string DetermineExperienceLevel(List<Entities.CandidateSkill> skills, string aiAnalysis)
    {
        if (!skills.Any())
            return "Not specified";

        var avgLevel = skills.Average(s => s.Level);
        var maxLevel = skills.Max(s => s.Level);

        if (avgLevel >= 4 && maxLevel >= 5)
            return "Senior (5+ years experience)";
        if (avgLevel >= 3.5 && maxLevel >= 4)
            return "Senior (3-5 years experience)";
        if (avgLevel >= 2.5 && maxLevel >= 3)
            return "Mid-Level (2-4 years experience)";
        if (avgLevel >= 1.5 && maxLevel >= 2)
            return "Junior (0-2 years experience)";
            
        return "Entry Level";
    }

    public async Task<Entities.CVAnalysis?> GetAnalysisAsync(int candidateId)
    {
        var analyses = await _unitOfWork.CVAnalyses.FindAsync(a => a.CandidateId == candidateId);
        return analyses.FirstOrDefault();
    }
}
