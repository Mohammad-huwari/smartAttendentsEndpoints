using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartAttendents.Models;

namespace smartAttendents.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        string messageNotFound = $"No student with this id found";

        // get all students
        [HttpGet]   
        public async Task<ActionResult<IEnumerable<Students>>> GetStudents()
        {
            return await _context.Students.AsNoTracking().ToListAsync();
        }

        // get student by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Students>> GetStudentByID(int id)
        {
           var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
      
            return student;
        }

        // create student 
        [HttpPost]
        public async Task<ActionResult<Students>> CreateStudent(Students newStudent)
        {
            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudentByID)
                , new { id = newStudent.StudentID} ,
                newStudent); 

        }

        // Update Student
        [HttpPut("{id}")]
        public async Task<ActionResult<Students>> UpdateStudent(int id , Students updatedStudent)
        {
            if (id != updatedStudent.StudentID) return BadRequest();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound(new {message = messageNotFound });

            _context.Entry(student).CurrentValues.SetValues(updatedStudent);
            await _context.SaveChangesAsync();

            return Ok(updatedStudent);
        }

        // Delete a student 
        [HttpDelete("{id}")]
        public async Task<ActionResult<Students>> DeleteStudent(int id)
        {
            
            var student = await _context.Students.FindAsync(id);

            if (student == null) return NotFound(new {message = messageNotFound });

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Student {id} deleted successfully"});
        }

        // GET /api/Students/my/absences-count?classId=123
        [HttpGet("my/absences-count")]
        public async Task<IActionResult> GetMyAbsencesCount([FromQuery] int classId)
        {
            var studentIdStr = User.FindFirst("studentId")?.Value
                               ?? User.FindFirst("StudentID")?.Value; 
            if (string.IsNullOrWhiteSpace(studentIdStr) || !int.TryParse(studentIdStr, out var studentId))
                return Forbid();

            
            var enrolled = await _context.ClassEnrollments.AsNoTracking()
                .AnyAsync(e => e.ClassID == classId && e.StudentID == studentId);
            if (!enrolled)
                return Ok(new { classId, absencesCount = 0 }); 

            
            var count = await _context.AttendanceLogs.AsNoTracking()
                .Where(a => a.ClassID == classId && a.StudentID == studentId && a.AttendanceStatus != null)
                .CountAsync(a =>
                    a.AttendanceStatus!.Trim().ToLower() == "absent"   
                    || a.AttendanceStatus!.Trim() == "1"              
                    || a.AttendanceStatus!.Trim().Equals("غياب")       
                );

            return Ok(new { classId, absencesCount = count });
        }
    }
}
