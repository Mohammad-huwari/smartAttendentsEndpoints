using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartAttendents.Models;


namespace smartAttendents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClassesController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: api/Classes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Classes>>> GetClasses()
        {
            return await _context.Classes.AsNoTracking().ToListAsync();
        }

        // GET: api/Classes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Classes>> GetClass(int id)
        {
            var classItem = await _context.Classes.FindAsync(id);

            if (classItem == null)
            {
                return NotFound();
            }

            return classItem;
        }

        // POST: api/Classes
        [HttpPost]
        public async Task<ActionResult<Classes>> CreateClass(Classes newClass)
        {
            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClass), new { id = newClass.ClassID }, newClass);
        }

        // update class
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id , Classes updatedClass)
        {
            if (id != updatedClass.ClassID) return BadRequest();

            var targetClass = await _context.Classes.FindAsync(id);
            if (targetClass == null) return NotFound();

            _context.Entry(targetClass).CurrentValues.SetValues(updatedClass);

            await _context.SaveChangesAsync();

            return Ok(updatedClass);
        }

        // delete a class 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            
            var targetClass = await _context.Classes.FindAsync(id);
            if (targetClass == null ) return NotFound();

            _context.Classes.Remove(targetClass);
            await _context.SaveChangesAsync();  

            return NoContent();
        }
    }
}
