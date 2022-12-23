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
        public async Task<ActionResult<Assignment>> PostAssignment(Assignment assignment)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            assignment.Id = 0; // reset the id to avoid db conflict
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
        }

        [HttpPut]
        public async Task<ActionResult> PutAssignment(Assignment assignment)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (!AssignmentExists(assignment.Id)) {
                return NotFound();
            }

            _context.Entry(assignment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return Ok();
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
            if (!AssignmentExists(assignmentId) || !_context.Projects.Any(x => x.Id == projectId))
            {
                return NotFound();
            }

            var assignment = _context.Assignments.Single(x => x.Id == assignmentId);

            assignment.ParentProjectId = projectId;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("ViewProjectAssignments")]
        public async Task<ActionResult<IEnumerable<Assignment>>> ViewProjectAssignments(int projectId)
        {
            if (!_context.Projects.Any(x => x.Id == projectId))
            {
                return NotFound();
            }

            return await _context.Assignments.Where(x => x.ParentProjectId == projectId).ToListAsync();
        }


        private bool AssignmentExists(int id) => _context.Assignments.Any(x => x.Id == id);
    }
}
