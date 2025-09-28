using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartAttendents.Models;

namespace smartAttendents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceLogController : Controller
    {

        private readonly AppDbContext _context;

        public AttendanceLogController(AppDbContext context)
        {
            _context = context;
        }

        // get all 
        [HttpGet]
        public async Task<ActionResult<AttendanceLog>> getAttendanceLogs()
        {
            var Att = await _context.AttendanceLogs.AsNoTracking().ToListAsync();

            return Ok(new { message = "data fetched successfully", data = Att });
        }

        // get by id 
        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceLog>> getAttendanceLogById(int id)
        {
            var Att = await _context.AttendanceLogs.FindAsync(id);
            if (Att == null) return NotFound(new { message = $"AttendanceLog with id {id} dosent exist" });

            return Ok(new {
                message = $"AttendanceLog with id {id} : ",
                data = Att });
        }

        // create
        [HttpPost]
        public async Task<ActionResult<AttendanceLog>> createAttendanceLog(AttendanceLog newAtt)
        {

            _context.AttendanceLogs.Add(newAtt);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(getAttendanceLogById) ,
                new {id = newAtt.AttendanceID} ,
                new { message = "AttendanceLog created successfully :" ,
                data = newAtt
                });

          
        }

        // update 
        [HttpPut("{id}")]
        public async Task<ActionResult<AttendanceLog>> updateAttendanceLog(int id, AttendanceLog updatedAtt)
        {
            if (id != updatedAtt.AttendanceID) return BadRequest(new { message = "invalid id input " });
            var Att = await _context.AttendanceLogs.FindAsync(id);

            if (Att == null) return NotFound(new { message = $"AttD with id {id} Not found" });

            _context.Entry(Att).CurrentValues.SetValues(updatedAtt);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Attd with id {id} was updated successfully" , data = updatedAtt });        
        }

        // delete
        [HttpDelete("{id}")]
        public async Task<ActionResult<AttendanceLog>> deleteAttendanceLog(int id)
        {
            var Att = await _context.AttendanceLogs.FindAsync(id);
            if (Att == null) return NotFound(new { message = $"Att with id {id} not found"});

            _context.AttendanceLogs.Remove(Att);
            await _context.SaveChangesAsync();

            return Ok(new {message = $"Att with id {id} was deleted successfully "});
        }

       
    }


}
