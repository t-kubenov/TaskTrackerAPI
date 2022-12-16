using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Data;

namespace TaskTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApiContext _context;
        public ProjectsController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) return NotFound();
            
            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        //[HttpPost("FromBody")]
        //public async Task<ActionResult<Project>> PostBody([FromBody] Project project) => await Post(project);

        [HttpPut]
        public async Task<ActionResult> PutProject(Project project)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (!_context.Projects.Any(x => x.Id == project.Id)) {
                return NotFound();
            }

            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null) {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
