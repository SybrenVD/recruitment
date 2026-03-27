using Microsoft.AspNetCore.Mvc;
using Recruitment.Requests;
using Recruitment.Responses;

namespace Recruitment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<SkillResponse>> GetAll()
        {
            return Ok(Array.Empty<SkillResponse>());
        }

        [HttpGet("{id}")]
        public ActionResult<SkillResponse> GetById(int id)
        {
            return NotFound();
        }

        [HttpPost]
        public ActionResult<SkillResponse> Create([FromBody] CreateSkillRequest request)
        {
            var response = new SkillResponse
            {
                Id = 1,
                SkillName = request.SkillName,
                Category = request.Category
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
