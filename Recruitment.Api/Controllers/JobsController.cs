using Microsoft.AspNetCore.Mvc;
using Recruitment.Interfaces;

namespace Recruitment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? location = null,
        [FromQuery] string? experienceLevel = null)
    {
        var jobs = await _jobService.GetAllAsync(searchTerm, location, experienceLevel);
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var job = await _jobService.GetByIdAsync(id);
        if (job == null)
            return NotFound();
        return Ok(job);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Entities.Job job)
    {
        var created = await _jobService.CreateAsync(job);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    public class CreateJobRequest
    {
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ExperienceLevel { get; set; }
        public int RecruiterId { get; set; }
        public List<SkillInput>? Skills { get; set; }
    }

    public class SkillInput
    {
        public string Name { get; set; } = "";
        public int Level { get; set; }
    }

    [HttpPost("create-with-skills")]
    public async Task<IActionResult> CreateWithSkills([FromBody] CreateJobRequest request)
    {
        var job = new Entities.Job
        {
            Title = request.Title,
            Description = request.Description,
            Location = request.Location,
            ExperienceLevel = request.ExperienceLevel,
            RecruiterId = request.RecruiterId
        };

        var skills = request.Skills?.Select(s => (s.Name, s.Level)).ToList() ?? new List<(string, int)>();
        var created = await _jobService.CreateWithSkillsAsync(job, skills);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Entities.Job job)
    {
        var existing = await _jobService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        job.Id = id;
        await _jobService.UpdateAsync(job);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _jobService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _jobService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/skills")]
    public async Task<IActionResult> GetSkills(int id)
    {
        var skills = await _jobService.GetSkillsAsync(id);
        return Ok(skills);
    }

    [HttpPost("{id}/skills")]
    public async Task<IActionResult> AddSkill(int id, [FromBody] Entities.JobSkill jobSkill)
    {
        await _jobService.AddSkillAsync(id, jobSkill);
        return Ok();
    }

    [HttpDelete("{id}/skills/{skillId}")]
    public async Task<IActionResult> RemoveSkill(int id, int skillId)
    {
        await _jobService.RemoveSkillAsync(id, skillId);
        return NoContent();
    }

    [HttpGet("{id}/matches")]
    public async Task<IActionResult> GetMatches(int id)
    {
        var matches = await _jobService.GetMatchesAsync(id);
        return Ok(matches);
    }

    [HttpGet("available/{candidateId}")]
    public async Task<IActionResult> GetAvailableForCandidate(int candidateId)
    {
        var jobs = await _jobService.GetAvailableForCandidateAsync(candidateId);
        return Ok(jobs);
    }
}
