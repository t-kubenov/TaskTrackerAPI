using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Data;
using Microsoft.IdentityModel.Tokens;

namespace TaskTrackerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApiContext _context;
        public ProjectsController(ApiContext context)
        {
            _context = context;
        }
        

        /// <summary>
        /// Gets all projects
        /// </summary>
        /// <returns>Returns a list of all projects in the database</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        /// <summary>
        /// Retrieves a project by ID
        /// </summary>
        /// <param name="projectId">ID of a project to retsrieve</param>
        /// <returns>Object of a sought-for project</returns>
        [HttpGet("{projectId}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProject(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null) return NotFound();

            return Ok(project);
        }

        /// <summary>
        /// Creates a project
        /// </summary>
        /// <param name="projectBody">All Project properties, except for the Id</param>
        /// <remarks>The Name parameter is required.
        /// <br>The StartDate should be strictly less than CompletionDate, their default values are set to today and one month from today respectively.</br>
        /// <br>Status' value range is limited in the range from 0 to 2, meaning NotStarted, Active, Completed respectively.</br>
        /// <br>Priority's range is 0 to 10.</br>
        /// </remarks>
        /// <returns>The new project item</returns>
        /// <response code="201">Returns the newly created project</response>
        /// <response code="400">If the entered data is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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


        /// <summary>
        /// Updates a project
        /// </summary>
        /// <param name="projectId">The Id of an existing project to update</param>
        /// <param name="projectBody">Properties to replace in a project with specified Id</param>
        /// <remarks>The Name parameter is required.
        /// <br>The StartDate should be strictly less than CompletionDate, their default values are set to today and one month from today respectively.</br>
        /// <br>Status' value range is limited in the range from 0 to 2, meaning NotStarted, Active, Completed respectively.</br>
        /// <br>Priority's range is 0 to 10.</br>
        /// </remarks>
        /// <returns>Updated project item</returns>
        /// <response code="201">Returns the updated project</response>
        /// <response code="400">If the entered data is invalid</response>
        /// <response code="404">If the project to update is not found</response>
        [HttpPut("{projectId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Deletes a project
        /// </summary>
        /// <param name="projectId">The Id of a project to delete</param>
        /// <remarks>Also modifies assignments of this project by setting their parent project Id to 0</remarks>
        /// <response code="200">Project deleted successfully</response>
        /// <response code="404">If the project to delete is not found</response>
        [HttpDelete("{projectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProject(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null) {
                return NotFound();
            }

            // Modify assignments that have deleted project as their parent project
            var affectedAssignments = _context.Assignments.Where(x => x.ParentProjectId == projectId);
            foreach (Assignment assignment in affectedAssignments)
            {
                assignment.ParentProjectId = 0;
            }


            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Filters existing projects by the entered parameters
        /// </summary>
        /// <param name="searchString">Search string in project names. Case insensitive</param>
        /// <param name="startDate">Exact start date. Overrides minStartDate and maxStartDate</param>
        /// <param name="minStartDate">Lower bound for start date. Non-strict</param>
        /// <param name="maxStartDate">Upper bound for start date. Non-strict</param>
        /// <param name="completionDate">Exact completion date. Overrides minCompletionDate and maxCompletionDate</param>
        /// <param name="minCompletionDate">Lower bound for completion date. Non-strict</param>
        /// <param name="maxCompletionDate">Upper bound for completion date. Non-strict</param>
        /// <param name="status">Exact status Id. 0 for Not Started, 1 for Active, 2 for Completed</param>
        /// <param name="priority">Exact priority value, ranges from 0 to 10. Overrides minPriority and maxPriority</param>
        /// <param name="minPriority">Upper bound for priority. Non-strict</param>
        /// <param name="maxPriority">Lower bound for priority. Non-strict</param>
        /// <remarks>All parameters are nullable, running the method without them returns all projects.</remarks>
        /// <returns>List of filtered Project entities</returns>
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
                result = result.Where( x => x.Name.Contains(searchString!) ); // Forgiving null because null check already exists.
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


        // Simple action to convert AssignmentBody to Assignment
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
