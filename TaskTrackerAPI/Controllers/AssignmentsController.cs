using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Data;

namespace TaskTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly ApiContext _context;
        public AssignmentsController(ApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all assignments
        /// </summary>
        /// <returns>Returns a list of all assignments in the database</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignments()
        {
            return await _context.Assignments.ToListAsync();
        }

        /// <summary>
        /// Retrieves an assignment by ID
        /// </summary>
        /// <param name="assignmentId">ID of an assignment to retrieve</param>
        /// <returns>Object of a sought-for assignment</returns>
        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignment(int assignmentId)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentId);

            if (assignment == null) return NotFound();
            
            return Ok(assignment);
        }

        /// <summary>
        /// Creates an assignment
        /// </summary>
        /// <param name="assignmentBody">All Assignment properties, except for the Id</param>
        /// <remarks>The Name parameter is required.</remarks>
        /// <returns>The new assignment item</returns>
        /// <response code="201">Returns the newly created assignment</response>
        /// <response code="400">If the entered data is invalid</response>
        [HttpPost]
        public async Task<ActionResult<Assignment>> PostAssignment(AssignmentBody assignmentBody)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            if (assignmentBody.ParentProjectId != 0 && !ProjectExists(assignmentBody.ParentProjectId))
            {
                return BadRequest("Invalid Parent Project ID");
            }

            Assignment assignment = BodyToAssignment(assignmentBody);

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAssignment), new { assignmentId = assignment.Id }, assignment);
        }

        /// <summary>
        /// Updates an assignment
        /// </summary>
        /// <param name="assignmentId">The Id of an existing assignment to update</param>
        /// <param name="assignmentBody">Properties to replace in an assignment with specified Id</param>
        /// <remarks>Parent Project Id can be set to 0 to remove the assignment from a project</remarks>
        /// <returns>Updated assignment item</returns>
        /// <response code="201">Returns the updated assignment</response>
        /// <response code="400">If the entered data is invalid</response>
        /// <response code="404">If the assignment to update is not found</response>
        [HttpPut("{assignmentId}")]
        public async Task<ActionResult> PutAssignment(int assignmentId, AssignmentBody assignmentBody)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (!AssignmentExists(assignmentId)) {
                return NotFound();
            }

            if (assignmentBody.ParentProjectId != 0 && !ProjectExists(assignmentBody.ParentProjectId))
            {
                return BadRequest("Invalid Parent Project ID");
            }

            Assignment assignment = BodyToAssignment(assignmentBody, assignmentId);

            _context.Entry(assignment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return Ok(assignment);
        }

        /// <summary>
        /// Deletes an assignment
        /// </summary>
        /// <param name="assignmentId">The Id of an assignment to delete</param>
        /// <response code="200">Assignment deleted successfully</response>
        /// <response code="404">If the assignment to delete is not found</response>
        [HttpDelete("{assignmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAssignment(int assignmentId)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentId);

            if (assignment == null) {
                return NotFound();
            }

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Assigns a parent project
        /// </summary>
        /// <param name="assignmentId">The assignment in which the Parent Project Id should be modified</param>
        /// <param name="projectId">Parent Project Id to set</param>
        /// <returns>The modified assignment</returns> 
        /// <remarks>Parent Project Id can be set to 0 to remove the assignment from a project</remarks>
        /// <response code="201">Returns the updated assignment</response>
        /// <response code="404">If the assignment to update is not found</response>
        [HttpPut("AssignParentProject")]
        public async Task<ActionResult> AssignParentProject(int assignmentId, int projectId)
        {
            if (!AssignmentExists(assignmentId) || (projectId != 0 && !ProjectExists(projectId)))
            {
                return NotFound();
            }

            var assignment = _context.Assignments.Single(x => x.Id == assignmentId);

            assignment.ParentProjectId = projectId;
            await _context.SaveChangesAsync();

            return Ok(assignment);
        }

        /// <summary>
        /// View Assignments tied to a Project, ordered by priority
        /// </summary>
        /// <param name="projectId">Id of the project</param>
        /// <remarks>0 can be entered as parentId to see assignments without a parent project</remarks>
        /// <returns>List of assignments from the project</returns>
        /// <response code="404">Project with entered Id is not found</response>
        [HttpGet("ViewProjectAssignments")]
        public async Task<ActionResult<IEnumerable<Assignment>>> ViewProjectAssignments(int projectId)
        {
            if (projectId != 0 && !ProjectExists(projectId))
            {
                return NotFound();
            }

            return await _context.Assignments.Where(x => x.ParentProjectId == projectId).OrderByDescending(x => x.Priority).ToListAsync();
        }


        private bool AssignmentExists(int id) => _context.Assignments.Any(x => x.Id == id);


        private bool ProjectExists(int id) => _context.Projects.Any(x => x.Id == id);


        // Simple action to convert AssignmentBody to Assignment
        private Assignment BodyToAssignment(AssignmentBody assignmentBody, int assignmentId = 0)
        {
            Assignment assignment = new()
            {
                Id = assignmentId,
                Name = assignmentBody.Name,
                Status = assignmentBody.Status,
                Description = assignmentBody.Description,
                Priority = assignmentBody.Priority,
                ParentProjectId= assignmentBody.ParentProjectId,
            };

            return assignment;
        }
    }
}
