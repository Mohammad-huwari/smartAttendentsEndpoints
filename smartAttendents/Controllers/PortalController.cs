using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartAttendents;            // AppDbContext
using smartAttendents.Models;     // Models
using System.Data;
using System.Security.Claims;

namespace smartAttendents.Controllers
{
    [ApiController]
    [Route("api/portal")]
    public class PortalController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PortalController(AppDbContext db) => _db = db;

        private bool TryGetIntClaim(string claimName, out int value)
        {
            value = 0;
            var str = User.FindFirst(claimName)?.Value;
            return !string.IsNullOrWhiteSpace(str) && int.TryParse(str, out value);
        }


        // get the classes of specific student by his id 
        [HttpGet("students/{studentId:int}/classes")]
        public async Task<IActionResult> GetStudentClasses(int studentId)
        {
            
            var data = await _db.ClassEnrollments
                .Where(e => e.StudentID == studentId)
                .Join(_db.Classes,
                      e => e.ClassID,
                      c => c.ClassID,
                      (e, c) => new
                      {
                          c.ClassID,
                          c.CourseCode,
                          c.ClassTime,
                          c.Room,
                          c.InstructorID
                      })
                .OrderBy(x => x.ClassTime)
                .ToListAsync();

            return Ok(new { message = "OK", data });
        }

       
        [HttpGet("instructors/{instructorId:int}/classes")]
        public async Task<IActionResult> GetInstructorClasses(int instructorId)
        {
          
            var data = await _db.Classes
                .Where(c => c.InstructorID == instructorId)
                .Select(c => new
                {
                    c.ClassID,
                    c.CourseCode,
                    c.ClassTime,
                    c.Room
                })
                .OrderBy(x => x.ClassTime)
                .ToListAsync();

            return Ok(new { message = "OK", data });
        }

        

        // get the students of specicfic class by his id
        [HttpGet("classes/{classId:int}/students")]
        public async Task<IActionResult> GetClassStudents(int classId)
        {
            var classExists = await _db.Classes.AnyAsync(c => c.ClassID == classId);
            if (!classExists)
                return NotFound(new { message = "Class not found" });

            var data = await _db.ClassEnrollments
                .Where(e => e.ClassID == classId)
                .Join(_db.Students,
                      e => e.StudentID,
                      s => s.StudentID,
                      (e, s) => new
                      {
                          s.StudentID,
                          s.FullName,
                          s.Email,
                          s.UniversityID,
                          s.IsActive
                      })
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return Ok(new { message = "OK", data });
        }

        // =========================
        //  NEW: STUDENT (/me) — Authorized
        // =========================
        // لا نمرّر IDs من الفرونت؛ نقرأ studentId من الـ token claim
        [Authorize(Roles = "student")]
        [HttpGet("students/me/classes")]
        public async Task<IActionResult> GetMyStudentClasses()
        {
            if (!TryGetIntClaim("studentId", out var studentId))
                return Forbid(); // التوكن لا يحتوي studentId

            var data = await _db.ClassEnrollments
                .Where(e => e.StudentID == studentId)
                .Join(_db.Classes,
                      e => e.ClassID,
                      c => c.ClassID,
                      (e, c) => new
                      {
                          c.ClassID,
                          c.CourseCode,
                          c.ClassTime,
                          c.Room,
                          c.InstructorID
                      })
                .OrderBy(x => x.ClassTime)
                .ToListAsync();

            return Ok(new { message = "OK", data });
        }

        // =========================
        //  NEW: INSTRUCTOR (/me) — Authorized
        // =========================
        // لا نمرّر IDs من الفرونت؛ نقرأ instructorId من الـ token claim
        [Authorize(Roles = "instructor")]
        [HttpGet("instructors/me/classes")]
        public async Task<IActionResult> GetMyInstructorClasses()
        {
            if (!TryGetIntClaim("instructorId", out var instructorId))
                return Forbid(); // التوكن لا يحتوي instructorId

            var data = await _db.Classes
                .Where(c => c.InstructorID == instructorId)
                .Select(c => new
                {
                    c.ClassID,
                    c.CourseCode,
                    c.ClassTime,
                    c.Room
                })
                .OrderBy(x => x.ClassTime)
                .ToListAsync();

            return Ok(new { message = "OK", data });
        }

        // =========================
        //  NEW: ADMIN (/me) — Authorized (placeholder)
        // =========================
        // Endpoint بسيط للفحص السريع — يرجّع بعض معلومات الـ claims
        [Authorize(Roles = "admin")]
        [HttpGet("admin/me")]
        public IActionResult GetMyAdminInfo()
        {
            var id = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name;
            var role = User.FindFirst("role")?.Value ?? "admin";

            return Ok(new
            {
                message = "OK",
                data = new { id, username, role }
            });
        }
    }
}
