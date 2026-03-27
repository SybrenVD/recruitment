using Microsoft.AspNetCore.Mvc;
using Recruitment.Interfaces;

namespace Recruitment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecruitersController : ControllerBase
{
    private readonly IRecruiterService _recruiterService;

    public RecruitersController(IRecruiterService recruiterService)
    {
        _recruiterService = recruiterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var recruiters = await _recruiterService.GetAllAsync();
        return Ok(recruiters);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var recruiter = await _recruiterService.GetByIdAsync(id);
        if (recruiter == null)
            return NotFound();
        return Ok(recruiter);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var recruiter = await _recruiterService.GetByEmailAsync(email);
        if (recruiter == null)
            return NotFound();
        return Ok(recruiter);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Entities.Recruiter recruiter)
    {
        var created = await _recruiterService.CreateAsync(recruiter);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Entities.Recruiter recruiter)
    {
        var existing = await _recruiterService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        recruiter.Id = id;
        await _recruiterService.UpdateAsync(recruiter);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _recruiterService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _recruiterService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/jobs")]
    public async Task<IActionResult> GetJobs(int id)
    {
        var jobs = await _recruiterService.GetJobsAsync(id);
        return Ok(jobs);
    }
}
