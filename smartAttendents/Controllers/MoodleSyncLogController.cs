using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartAttendents.Models;

namespace smartAttendents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoodleSyncLogController : Controller
    {
        private readonly AppDbContext _context;

        public MoodleSyncLogController(AppDbContext context)
        {
            _context = context;
        }

        // get all
        [HttpGet]
        public async Task<ActionResult<MoodleSyncLog>> GetAllMoodleLogs()
        {
            var Logs = await _context.MoodleSyncLog.AsNoTracking().ToListAsync();
            return Ok(new { message = "Data was fetched successfully ", data = Logs });
        }


        // get by id 
        [HttpGet("{id}")]
        public async Task<ActionResult<MoodleSyncLog>> GetOneMoodleLog(int id)
        {
            var Log = await _context.MoodleSyncLog.FindAsync(id);
            if (Log == null) return NotFound(new { message = $"No log with the id {id}" });

            return Ok(new {message = $"the log with id {id}" , data = Log});
        }

        // create one 
        [HttpPost]
        public async Task<ActionResult<MoodleSyncLog>> CreateMoodleLog(MoodleSyncLog newMoodleLog)
        {
            if (newMoodleLog.AttendanceID.HasValue)
            {
                var att = await _context.AttendanceLogs.
                    AnyAsync(a => a.AttendanceID == newMoodleLog.AttendanceID.Value);

                if (!att)
                {
                    return BadRequest(new { message = $"No AttendanceLog with this id" });
                }
            }

            _context.MoodleSyncLog.Add(newMoodleLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOneMoodleLog),
                new {id = newMoodleLog.SyncID } ,
                new {message = "Log created successfully" ,
                    data = newMoodleLog 
                });

            //return Ok(new { message = "Log created successfully" , data = newMoodleLog});
        }

        // Update 
        [HttpPut("{id}")]
        public async Task<ActionResult<MoodleSyncLog>> UpdateMoodleLog( int id, MoodleSyncLog updetedMoodleLog)
        {
            if (id != updetedMoodleLog.SyncID) return BadRequest();

            if (updetedMoodleLog.AttendanceID.HasValue)
            {
                var att = await _context.AttendanceLogs.
                    AnyAsync(a => a.AttendanceID == updetedMoodleLog.AttendanceID.Value);

                if (!att)
                {
                    return BadRequest(new { message = $"No AttendanceLog with this id" });
                }
            }


            var Log = await _context.MoodleSyncLog.FindAsync(id);
            if (Log == null) return NotFound(new {message = $"No log found with this id {id}"});

            _context.Entry(Log).CurrentValues.SetValues(updetedMoodleLog);
            await _context.SaveChangesAsync();

            return Ok(new { message = "MoodleLog updated successfully" , data = updetedMoodleLog});    
        }

        //Delete
        [HttpDelete("{id}")]
        public async Task<ActionResult<MoodleSyncLog>> DeletMoodleLog(int id)
        {
            var Log = await _context.MoodleSyncLog.FindAsync(id);
            if (Log == null) return NotFound(new { message = $"No Log fond with this id {id}" });

            _context.MoodleSyncLog.Remove(Log);
            await _context.SaveChangesAsync();

            return Ok(new { messaeg = $"The Moodle Log with id {id} was Deleted successfully"});
        }


    }
}
