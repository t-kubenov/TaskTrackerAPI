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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignments()
        {
            return await _context.Assignments.ToListAsync();
        }

        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignment(int assignmentId)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentId);

            if (assignment == null) return NotFound();
            
            return Ok(assignment);
        }

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

        [HttpDelete("{assignmentId}")]
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

        [HttpPut("AssignParentProject")]
        public async Task<ActionResult> AssignParentProject(int assignmentId, int projectId)
        {
            if (!AssignmentExists(assignmentId) || !ProjectExists(projectId))
            {
                return NotFound();
            }

            var assignment = _context.Assignments.Single(x => x.Id == assignmentId);

            assignment.ParentProjectId = projectId;
            await _context.SaveChangesAsync();

            return Ok(assignment);
        }

        [HttpGet("ViewProjectAssignments")]
        public async Task<ActionResult<IEnumerable<Assignment>>> ViewProjectAssignments(int projectId)
        {
            if (!_context.Projects.Any(x => x.Id == projectId))
            {
                return NotFound();
            }

            return await _context.Assignments.Where(x => x.ParentProjectId == projectId).OrderByDescending(x => x.Priority).ToListAsync();
        }


        private bool AssignmentExists(int id) => _context.Assignments.Any(x => x.Id == id);


        private bool ProjectExists(int id) => _context.Projects.Any(x => x.Id == id);


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
