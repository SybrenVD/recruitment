using Microsoft.AspNetCore.Mvc;
using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<JobResponse>> GetAll()
        {
            return Ok(Array.Empty<JobResponse>());
        }

        [HttpGet("{id}")]
        public ActionResult<JobResponse> GetById(int id)
        {
            return NotFound();
        }

        [HttpPost]
        public ActionResult<JobResponse> Create([FromBody] CreateJobRequest request)
        {
            var response = new JobResponse
            {
                Id = 1,
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                ExperienceLevel = request.ExperienceLevel,
                EducationLevel = request.EducationLevel,
                RecruiterId = request.RecruiterId,
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
