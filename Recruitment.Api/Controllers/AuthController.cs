using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Recruitment.Entities;
using Recruitment.Interfaces;

namespace Recruitment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ICandidateService _candidateService;
    private readonly IRecruiterService _recruiterService;
    private readonly IConfiguration _config;

    public AuthController(ICandidateService candidateService, IRecruiterService recruiterService, IConfiguration config)
    {
        _candidateService = candidateService;
        _recruiterService = recruiterService;
        _config = config;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == hash;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Password))
            return BadRequest(new { error = "Password is required" });

        if (request.UserType == "candidate")
        {
            var existing = await _candidateService.GetByEmailAsync(request.Email);
            if (existing != null)
                return BadRequest(new { error = "Email already registered" });

            var candidate = new Entities.Candidate
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Location = request.Location,
                CreatedAt = DateTime.UtcNow
            };
            await _candidateService.CreateAsync(candidate);
        }
        else if (request.UserType == "recruiter")
        {
            var existing = await _recruiterService.GetByEmailAsync(request.Email);
            if (existing != null)
                return BadRequest(new { error = "Email already registered" });

            var recruiter = new Entities.Recruiter
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Company = request.Company ?? "",
                CreatedAt = DateTime.UtcNow
            };
            await _recruiterService.CreateAsync(recruiter);
        }
        else
        {
            return BadRequest(new { error = "Invalid user type" });
        }

        return Ok(new { success = true });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        object? user = null;
        string? role = null;
        int? userId = null;
        string? passwordHash = null;

        if (request.UserType == "candidate")
        {
            var candidate = await _candidateService.GetByEmailAsync(request.Email);
            if (candidate != null)
            {
                user = candidate;
                role = "Candidate";
                userId = candidate.Id;
                passwordHash = candidate.PasswordHash;
            }
        }
        else if (request.UserType == "recruiter")
        {
            var recruiter = await _recruiterService.GetByEmailAsync(request.Email);
            if (recruiter != null)
            {
                user = recruiter;
                role = "Recruiter";
                userId = recruiter.Id;
                passwordHash = recruiter.PasswordHash;
            }
        }

        if (user == null || role == null)
            return Unauthorized(new { error = "Invalid email or user type" });

        if (string.IsNullOrEmpty(passwordHash) || !VerifyPassword(request.Password, passwordHash))
            return Unauthorized(new { error = "Invalid password" });

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.Role, role),
            new Claim("UserId", userId?.ToString() ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "THIS IS A DEV SECRET"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "Recruitment.Api",
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { token = jwt, role, userId });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string UserType { get; set; } = "candidate";
    public string? Company { get; set; }
}
