using Microsoft.AspNetCore.Mvc;
using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecruitersController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<RecruiterResponse>> GetAll()
        {
            return Ok(Array.Empty<RecruiterResponse>());
        }

        [HttpGet("{id}")]
        public ActionResult<RecruiterResponse> GetById(int id)
        {
            return NotFound();
        }

        [HttpPost]
        public ActionResult<RecruiterResponse> Create([FromBody] CreateRecruiterRequest request)
        {
            var response = new RecruiterResponse
            {
                Id = 1,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Company = request.Company,
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
