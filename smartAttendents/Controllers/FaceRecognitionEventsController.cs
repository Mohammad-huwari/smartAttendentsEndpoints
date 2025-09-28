using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartAttendents.Models;

namespace smartAttendents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaceRecognitionEventsController : Controller
    {
        private readonly AppDbContext _context;

        public FaceRecognitionEventsController(AppDbContext context)
        {
            _context = context;
        }

        // Get All 
        [HttpGet]
        public async Task<ActionResult<FaceRecognitionEvents>> GetAllFaceRecEvent()
        {
            var targets = await _context.FaceRecognitionEvents.AsNoTracking().ToListAsync();
            return Ok(new {
                messsage = "data was fetched successfully " ,
                data = targets
            });
        }

        //Get by id
        [HttpGet("{id}")]
        public async Task<ActionResult<FaceRecognitionEvents>> GetOneFaceRecEvent(int id )
        {
            var target = await _context.FaceRecognitionEvents.FindAsync(id);
            if(target == null) return NotFound(new { message = $"No Event found with this id"});

            return Ok(new { 
                message = $"Event with id {id} was fetched successfully : ",
                data = target
            });

        }

        // create 
        [HttpPost]
        public async Task<ActionResult<FaceRecognitionEvents>> CreateFaceRecEvent(FaceRecognitionEvents newEvent)
        {
            if (newEvent.StudentID.HasValue)
            {
                var studentExist = await _context.Students
                    .AnyAsync(s => s.StudentID == newEvent.StudentID.Value);

                if (!studentExist)
                    return BadRequest(new { message = $"No student found with this id {newEvent.StudentID} " });
                
            }

            if(newEvent.ClassID.HasValue)
            {
                var existClass = await _context.Classes
                    .AnyAsync(c => c.ClassID == newEvent.ClassID.Value);

                if (!existClass)
                    return BadRequest(new { message = $"No class found with this id {newEvent.ClassID} " });
            }

            _context.FaceRecognitionEvents.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOneFaceRecEvent),
                new { id = newEvent.EventID } ,
                new {message = "Event created successfully : "  ,
                    data = newEvent
                });

        }

        // update 
        [HttpPut("{id}")]
        public async Task<ActionResult<FaceRecognitionEvents>> UpdateFaceRecEvent(int id, FaceRecognitionEvents updatedEvent)
        {
            if (id != updatedEvent.EventID) return BadRequest(new {message = "wrong input id"});

            if (updatedEvent.StudentID.HasValue)
            {
                var studentExist = await _context.Students
                    .AnyAsync(s => s.StudentID == updatedEvent.StudentID.Value);

                if (!studentExist)
                    return BadRequest(new { message = $"No student found with this id {updatedEvent.StudentID} " });

            }

            if (updatedEvent.ClassID.HasValue)
            {
                var existClass = await _context.Classes
                    .AnyAsync(c => c.ClassID == updatedEvent.ClassID.Value);

                if (!existClass)
                    return BadRequest(new { message = $"No class found with this id {updatedEvent.ClassID} " });
            }

            var targetEvent = await _context.FaceRecognitionEvents.FindAsync(id);
            if (targetEvent == null) return NotFound(new { message = $"No Event found with this id {id}" });

            _context.Entry(targetEvent).CurrentValues.SetValues(updatedEvent);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Event with id {id} updated successfully", data = updatedEvent });

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<FaceRecognitionEvents>> DeleteFaceRecEvent(int id)
        {
            var targetEvent = await _context.FaceRecognitionEvents.FindAsync(id);
            if (targetEvent == null) return NotFound(new { message = $"No Event found with this id {id}" });

            _context.FaceRecognitionEvents.Remove(targetEvent);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"The event with id {id} deleted successfully " });

        }


    }
}
