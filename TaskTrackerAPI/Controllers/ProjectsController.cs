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


        [HttpGet("{projectId}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProject(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null) return NotFound();

            return Ok(project);
        }


        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(ProjectBody projectBody)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!BodyDateValid(projectBody))
            {
                return BadRequest("Incorrect Completion Date");
            }

            Project project = BodyToProject(projectBody);

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { projectId = project.Id }, project);
        }


        [HttpPut("{projectId}")]
        public async Task<ActionResult> PutProject(int projectId, ProjectBody projectBody)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (!ProjectExists(projectId)) {
                return NotFound();
            }

            if (!BodyDateValid(projectBody))
            {
                return BadRequest("Incorrect Completion Date");
            }

            Project project = BodyToProject(projectBody, projectId);

            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return Ok(project);
        }


        [HttpDelete("{projectId}")]
        public async Task<ActionResult> DeleteProject(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null) {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok();
        }


        private bool ProjectExists(int id) => _context.Projects.Any(x => x.Id == id);


        private bool BodyDateValid(ProjectBody projectBody) => projectBody.StartDate < projectBody.CompletionDate;


        private Project BodyToProject(ProjectBody projectBody, int projectId = 0)
        {
            Project project = new()
            {
                Id = projectId,
                Name = projectBody.Name,
                StartDate = projectBody.StartDate,
                CompletionDate = projectBody.CompletionDate,
                Status = projectBody.Status,
                Priority = projectBody.Priority,
            };

            return project;
        }
    }
}
