using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartAttendents.Models;

namespace smartAttendents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassEnrollmentsController : Controller
    {
        private readonly AppDbContext _context;

        public ClassEnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        // get all 
        [HttpGet]
        public async Task<ActionResult<ClassEnrollment>> GetClassEnrollments()
        {
            var enrollments = await _context.ClassEnrollments.AsNoTracking().ToListAsync();
            return Ok(new { message = "data has been fetched successfully" , data = enrollments  });
        }

        // get by id
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassEnrollment>> GetOneEnrollment(int id)
        {
            var enrollment = await _context.ClassEnrollments.FindAsync(id);
            if (enrollment == null) return NotFound(new { message = $"enrollment with id {id} cannot be found" });

            return Ok(new { message = $"enrollment with id {id} : ", enrollment = enrollment });
        }

        // create Enrollment
        [HttpPost]
        public async Task<ActionResult<ClassEnrollment>> CreateEnrollment(ClassEnrollment newEnrollment)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.StudentID == newEnrollment.StudentID);
            if (!studentExists)
                return BadRequest(new { message = $"Student {newEnrollment.StudentID} does not exist." });

            var classExists = await _context.Classes.AnyAsync(c => c.ClassID == newEnrollment.ClassID);
            if (!classExists)
                return BadRequest(new { message = $"Class {newEnrollment.ClassID} does not exist." });

            // ✅ الشرط الجديد: منع التكرار (StudentID + ClassID)
            var alreadyEnrolled = await _context.ClassEnrollments
                .AnyAsync(e => e.ClassID == newEnrollment.ClassID && e.StudentID == newEnrollment.StudentID);

            if (alreadyEnrolled)
                return BadRequest(new { message = $"Student {newEnrollment.StudentID} is already enrolled in class {newEnrollment.ClassID}." });
            // (بدالك ممكن تستخدم Conflict() بدل BadRequest لو بتحب: return Conflict(new { message = ... });

            _context.ClassEnrollments.Add(newEnrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOneEnrollment),
                new { id = newEnrollment.EnrollmentID },
                new { message = "enrollment was created successfully :", data = newEnrollment });
        }


        // update enrollment 
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEnrollment(int id, [FromBody] UpdateEnrollmentDto dto)
        {
            var enrollment = await _context.ClassEnrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound(new { message = $"enrollment with id {id} not found" });

            // تحقق وجود المراجع
            if (!await _context.Students.AnyAsync(s => s.StudentID == dto.StudentID))
                return BadRequest(new { message = $"Student {dto.StudentID} does not exist." });

            if (!await _context.Classes.AnyAsync(c => c.ClassID == dto.ClassID))
                return BadRequest(new { message = $"Class {dto.ClassID} does not exist." });

            // منع تكرار (ClassID, StudentID) مع استثناء السجل الحالي
            bool duplicate = await _context.ClassEnrollments
                .AnyAsync(e => e.ClassID == dto.ClassID && e.StudentID == dto.StudentID && e.EnrollmentID != id);
            if (duplicate)
                return BadRequest(new { message = $"Student {dto.StudentID} is already enrolled in class {dto.ClassID}." });
            // أو: return Conflict(new { message = ... });

            // التحديث الحذِر (بدون SetValues حتى لا نلمس مفاتيح أخرى)
            enrollment.ClassID = dto.ClassID;
            enrollment.StudentID = dto.StudentID;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Enrollment was updated successfully",
                data = new
                {
                    enrollment.EnrollmentID,
                    enrollment.ClassID,
                    enrollment.StudentID,
                    enrollment.RowVersion
                }
            });
        }

        // Delete 
        [HttpDelete("{id}")]
        public async Task<ActionResult<ClassEnrollment>> DeleteEnrollment(int id)
        {
            var enrollment = await _context.ClassEnrollments.FindAsync(id);
            if (enrollment == null) return NotFound(new {message = $"No inrollment found with this id"}); 

            _context.ClassEnrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Enrollment with id {id} was deketed successfully" });
        }

    }
}
