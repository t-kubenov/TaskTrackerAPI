using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Data;
using Microsoft.IdentityModel.Tokens;

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

        //TODO: Fix Status
        [HttpGet("FilterProjects")]
        public ActionResult<IEnumerable<Project>> FilterProjects(
            string? searchString,
            DateTime? startDate,
            DateTime? minStartDate,
            DateTime? maxStartDate,
            DateTime? completionDate,
            DateTime? minCompletionDate,
            DateTime? maxCompletionDate,
            int? status,
            int? priority,
            int? minPriority,
            int? maxPriority
            )
        {
            var result = _context.Projects.AsQueryable();

            if (!searchString.IsNullOrEmpty())
            {
                result = result.Where( x => x.Name.Contains(searchString) );
            }

            if (startDate.HasValue) 
            {
                result = result.Where(x => x.StartDate == startDate);
            }

            else
            {
                if (minStartDate.HasValue)
                {
                    result = result.Where(x => x.StartDate >= minStartDate);
                }

                if (maxStartDate.HasValue)
                {
                    result = result.Where(x => x.StartDate <= maxStartDate);
                }
            }

            if (completionDate.HasValue)
            {
                result = result.Where(x => x.CompletionDate == completionDate);
            }

            else
            {
                if (minCompletionDate.HasValue)
                {
                    result = result.Where(x => x.CompletionDate >= minCompletionDate);
                }

                if (maxCompletionDate.HasValue)
                {
                    result = result.Where(x => x.CompletionDate <= maxCompletionDate);
                }
            }

            if (status.HasValue)
            {
                result = result.Where(x => (int)x.Status == status);
            }

            if (priority.HasValue)
            {
                result = result.Where(x => x.Priority == priority);
            }

            else
            {
                if (minPriority.HasValue)
                {
                    result = result.Where(x => x.Priority >= minPriority);
                }

                if (maxPriority.HasValue)
                {
                    result = result.Where(x => x.Priority <= maxPriority);
                }
            }


            return result.ToList();
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
