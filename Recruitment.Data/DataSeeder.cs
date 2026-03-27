using Recruitment.Data;
using Recruitment.Entities;

namespace Recruitment.Data;

public class DataSeeder
{
    private readonly RecruitmentDbContext _context;

    public DataSeeder(RecruitmentDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (_context.Skills.Any())
            return;

        var skills = new List<Skill>
        {
            new Skill { SkillName = "C#", Category = "Programming" },
            new Skill { SkillName = "JavaScript", Category = "Programming" },
            new Skill { SkillName = "Python", Category = "Programming" },
            new Skill { SkillName = "Java", Category = "Programming" },
            new Skill { SkillName = "TypeScript", Category = "Programming" },
            new Skill { SkillName = "Go", Category = "Programming" },
            new Skill { SkillName = "Rust", Category = "Programming" },
            new Skill { SkillName = "SQL", Category = "Database" },
            new Skill { SkillName = "PostgreSQL", Category = "Database" },
            new Skill { SkillName = "MongoDB", Category = "Database" },
            new Skill { SkillName = "Redis", Category = "Database" },
            new Skill { SkillName = "React", Category = "Frontend" },
            new Skill { SkillName = "Angular", Category = "Frontend" },
            new Skill { SkillName = "Vue.js", Category = "Frontend" },
            new Skill { SkillName = "Blazor", Category = "Frontend" },
            new Skill { SkillName = "ASP.NET Core", Category = "Backend" },
            new Skill { SkillName = "Node.js", Category = "Backend" },
            new Skill { SkillName = "Django", Category = "Backend" },
            new Skill { SkillName = "Spring Boot", Category = "Backend" },
            new Skill { SkillName = "Docker", Category = "DevOps" },
            new Skill { SkillName = "Kubernetes", Category = "DevOps" },
            new Skill { SkillName = "Azure", Category = "DevOps" },
            new Skill { SkillName = "AWS", Category = "DevOps" },
            new Skill { SkillName = "Git", Category = "Tools" },
            new Skill { SkillName = "Agile", Category = "Soft Skills" },
            new Skill { SkillName = "Scrum", Category = "Soft Skills" },
            new Skill { SkillName = "Communication", Category = "Soft Skills" },
            new Skill { SkillName = "Leadership", Category = "Soft Skills" }
        };

        _context.Skills.AddRange(skills);

        var recruiter = new Recruiter
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@techcorp.com",
            Company = "TechCorp Inc.",
            CreatedAt = DateTime.UtcNow
        };
        _context.Recruiters.Add(recruiter);

        var recruiter2 = new Recruiter
        {
            FirstName = "Sarah",
            LastName = "Johnson",
            Email = "sarah.j@startup.io",
            Company = "Startup.io",
            CreatedAt = DateTime.UtcNow
        };
        _context.Recruiters.Add(recruiter2);

        var candidate = new Candidate
        {
            FirstName = "Alice",
            LastName = "Brown",
            Email = "alice.brown@email.com",
            Phone = "+1-555-0101",
            Location = "New York, NY",
            CreatedAt = DateTime.UtcNow
        };
        _context.Candidates.Add(candidate);

        var candidate2 = new Candidate
        {
            FirstName = "Bob",
            LastName = "Wilson",
            Email = "bob.wilson@email.com",
            Phone = "+1-555-0102",
            Location = "San Francisco, CA",
            CreatedAt = DateTime.UtcNow
        };
        _context.Candidates.Add(candidate2);

        await _context.SaveChangesAsync();

        var cSharpSkill = skills.First(s => s.SkillName == "C#");
        var aspNetSkill = skills.First(s => s.SkillName == "ASP.NET Core");
        var sqlSkill = skills.First(s => s.SkillName == "SQL");
        var dockerSkill = skills.First(s => s.SkillName == "Docker");

        var job = new Job
        {
            Title = "Senior .NET Developer",
            Description = "We are looking for an experienced .NET developer to join our team.",
            Location = "New York, NY",
            ExperienceLevel = "Senior",
            EducationLevel = "Bachelor's",
            RecruiterId = recruiter.Id,
            CreatedAt = DateTime.UtcNow
        };
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        _context.JobSkills.AddRange(new[]
        {
            new JobSkill { JobId = job.Id, SkillId = cSharpSkill.Id, RequiredLevel = 4, Weight = 3 },
            new JobSkill { JobId = job.Id, SkillId = aspNetSkill.Id, RequiredLevel = 4, Weight = 3 },
            new JobSkill { JobId = job.Id, SkillId = sqlSkill.Id, RequiredLevel = 3, Weight = 2 },
            new JobSkill { JobId = job.Id, SkillId = dockerSkill.Id, RequiredLevel = 2, Weight = 1 }
        });

        _context.CandidateSkills.AddRange(new[]
        {
            new CandidateSkill { CandidateId = candidate.Id, SkillId = cSharpSkill.Id, Level = 5 },
            new CandidateSkill { CandidateId = candidate.Id, SkillId = aspNetSkill.Id, Level = 4 },
            new CandidateSkill { CandidateId = candidate.Id, SkillId = sqlSkill.Id, Level = 3 },
            new CandidateSkill { CandidateId = candidate2.Id, SkillId = cSharpSkill.Id, Level = 3 },
            new CandidateSkill { CandidateId = candidate2.Id, SkillId = sqlSkill.Id, Level = 4 }
        });

        await _context.SaveChangesAsync();
    }
}
