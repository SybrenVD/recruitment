using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Recruitment.Data;
using Recruitment.Interfaces;
using Recruitment.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RecruitmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IRecruiterService, RecruiterService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<ICVAnalysisService, CVAnalysisService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IPdfReaderService, PdfReaderService>();
builder.Services.AddScoped<IAIService, AIService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "THIS IS A DEV SECRET";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Recruitment.Api";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("http://localhost:5246", "https://localhost:5247", "http://localhost:5000", "https://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RecruitmentDbContext>();
    dbContext.Database.EnsureCreated();
    
    // Only seed skills if empty
    if (!dbContext.Skills.Any())
    {
        var skills = new[]
        {
            new Recruitment.Entities.Skill { SkillName = "C#", Category = "Programming" },
            new Recruitment.Entities.Skill { SkillName = "JavaScript", Category = "Programming" },
            new Recruitment.Entities.Skill { SkillName = "Python", Category = "Programming" },
            new Recruitment.Entities.Skill { SkillName = "Java", Category = "Programming" },
            new Recruitment.Entities.Skill { SkillName = "TypeScript", Category = "Programming" },
            new Recruitment.Entities.Skill { SkillName = "Go", Category = "Programming" },
            new Recruitment.Entities.Skill { SkillName = "SQL", Category = "Database" },
            new Recruitment.Entities.Skill { SkillName = "PostgreSQL", Category = "Database" },
            new Recruitment.Entities.Skill { SkillName = "React", Category = "Frontend" },
            new Recruitment.Entities.Skill { SkillName = "Angular", Category = "Frontend" },
            new Recruitment.Entities.Skill { SkillName = "Vue.js", Category = "Frontend" },
            new Recruitment.Entities.Skill { SkillName = "ASP.NET Core", Category = "Backend" },
            new Recruitment.Entities.Skill { SkillName = "Node.js", Category = "Backend" },
            new Recruitment.Entities.Skill { SkillName = "Docker", Category = "DevOps" },
            new Recruitment.Entities.Skill { SkillName = "Kubernetes", Category = "DevOps" },
            new Recruitment.Entities.Skill { SkillName = "AWS", Category = "DevOps" },
            new Recruitment.Entities.Skill { SkillName = "Git", Category = "Tools" },
            new Recruitment.Entities.Skill { SkillName = "Agile", Category = "Soft Skills" }
        };
        dbContext.Skills.AddRange(skills);
        dbContext.SaveChanges();
    }
}

app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
