using Microsoft.AspNetCore.Mvc;
using Recruitment.Interfaces;

namespace Recruitment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchingController : ControllerBase
{
    private readonly IMatchingService _matchingService;
    private readonly ICVAnalysisService _cvAnalysisService;

    public MatchingController(IMatchingService matchingService, ICVAnalysisService cvAnalysisService)
    {
        _matchingService = matchingService;
        _cvAnalysisService = cvAnalysisService;
    }

    [HttpPost("analyze-cv/{candidateId}")]
    public async Task<IActionResult> AnalyzeCV(int candidateId)
    {
        var analysis = await _cvAnalysisService.AnalyzeCVAsync(candidateId);
        return Ok(analysis);
    }

    [HttpGet("candidates/{candidateId}/suggestions")]
    public async Task<IActionResult> GetCandidateSuggestions(int candidateId, [FromQuery] int limit = 10)
    {
        var suggestions = await _matchingService.GetSuggestionsForCandidateAsync(candidateId, limit);
        return Ok(suggestions);
    }

    [HttpGet("jobs/{jobId}/suggestions")]
    public async Task<IActionResult> GetJobSuggestions(int jobId, [FromQuery] int limit = 10)
    {
        var suggestions = await _matchingService.GetSuggestionsForJobAsync(jobId, limit);
        return Ok(suggestions);
    }

    [HttpPost("swipe")]
    public async Task<IActionResult> Swipe([FromBody] SwipeRequest request)
    {
        await _matchingService.ProcessSwipeAsync(request.CandidateId, request.JobId, request.IsLike);
        return Ok();
    }

    [HttpGet("mutual")]
    public async Task<IActionResult> GetMutualMatches([FromQuery] int userId, [FromQuery] bool isCandidate)
    {
        var matches = await _matchingService.GetMutualMatchesAsync(userId, isCandidate);
        return Ok(matches);
    }
}

public class SwipeRequest
{
    public int CandidateId { get; set; }
    public int JobId { get; set; }
    public bool IsLike { get; set; }
}
