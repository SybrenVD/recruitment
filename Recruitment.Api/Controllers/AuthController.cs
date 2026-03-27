using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

    [HttpPost("login")] // POST: api/auth/login
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Demo: login op email, geen wachtwoord (voor demo, voeg wachtwoord toe voor productie!)
        object? user = null;
        string? role = null;
        if (request.UserType == "candidate")
        {
            var candidate = await _candidateService.GetByEmailAsync(request.Email);
            if (candidate != null)
            {
                user = candidate;
                role = "Candidate";
            }
        }
        else if (request.UserType == "recruiter")
        {
            var recruiter = await _recruiterService.GetByEmailAsync(request.Email);
            if (recruiter != null)
            {
                user = recruiter;
                role = "Recruiter";
            }
        }
        if (user == null || role == null)
            return Unauthorized();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.Role, role)
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
        return Ok(new { token = jwt, role });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty; // "candidate" of "recruiter"
}
