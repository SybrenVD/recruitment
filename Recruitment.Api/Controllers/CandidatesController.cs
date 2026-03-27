using Microsoft.AspNetCore.Mvc;
using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<CandidateResponse>> GetAll()
        {
            return Ok(Array.Empty<CandidateResponse>());
        }

        [HttpGet("{id}")]
        public ActionResult<CandidateResponse> GetById(int id)
        {
            return NotFound();
        }

        [HttpPost]
        public ActionResult<CandidateResponse> Create([FromBody] CreateCandidateRequest request)
        {
            var response = new CandidateResponse
            {
                Id = 1,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Location = request.Location,
                CreatedAt = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NoContent();
        }
    }
}
