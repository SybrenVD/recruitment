using Microsoft.AspNetCore.Mvc;
using Recruitment.Interfaces;

namespace Recruitment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var candidates = await _candidateService.GetAllAsync();
        return Ok(candidates);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var candidate = await _candidateService.GetByIdAsync(id);
        if (candidate == null)
            return NotFound();
        return Ok(candidate);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var candidate = await _candidateService.GetByEmailAsync(email);
        if (candidate == null)
            return NotFound();
        return Ok(candidate);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Entities.Candidate candidate)
    {
        var created = await _candidateService.CreateAsync(candidate);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Entities.Candidate candidate)
    {
        var existing = await _candidateService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        candidate.Id = id;
        await _candidateService.UpdateAsync(candidate);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _candidateService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _candidateService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/cv")]
    public async Task<IActionResult> UploadCV(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        using var stream = file.OpenReadStream();
        var path = await _candidateService.UploadCVAsync(id, stream, file.FileName);
        return Ok(new { path });
    }

    [HttpGet("{id}/cv")]
    public async Task<IActionResult> GetCV(int id)
    {
        var stream = await _candidateService.GetCVAsync(id);
        if (stream == null)
            return NotFound();

        return File(stream, "application/pdf");
    }
}
