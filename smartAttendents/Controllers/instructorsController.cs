using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartAttendents.Models;

namespace smartAttendents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorsController : Controller
    {
        private readonly AppDbContext _context;

        public InstructorsController(AppDbContext context)
        {
            _context = context;
        }

        // get all constructors
        [HttpGet]
        public async Task<ActionResult<Instructors>> getInstructors()
        {
            var instructors = await _context.Instructors.AsNoTracking().ToListAsync();

            return Ok(new { message =  "data fetched successfully" , data = instructors });
        }

        // get one
        [HttpGet("{id}")]
        public async Task<ActionResult<Instructors>> getInstructorById(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound(new {message = $"instroctur with id {id} not found." });

            return Ok(new {
                message = $"instroctur with id {id} : "
                , data = instructor } );

        }

        // create 
        [HttpPost]
        public async Task<ActionResult<Instructors>> createInstructor(Instructors newInstructor) 
        {
            _context.Instructors.Add(newInstructor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getInstructorById) ,
                new {id = newInstructor.InstructorID} ,
                new {message = "instructor has been added successfully" ,
                    data = newInstructor
                });

           
        }

        // update
        [HttpPut("{id}")]
        public async Task<ActionResult<Instructors>> updateInstructor(int id , Instructors updatedInstructor)
        {
            if (id != updatedInstructor.InstructorID) return BadRequest(new { message = "Wrong id input" });
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null) return NotFound(new {message = "No instructor found with this id !"});

            _context.Entry(instructor).CurrentValues.SetValues(updatedInstructor);
            await _context.SaveChangesAsync();

            return Ok(new {
                message = $"informations of the instructor with id {id} updated successfully "
                , updatedInstructor });

        }


        // delete one 
        [HttpDelete("{id}")]
        public async Task<ActionResult<Instructors>> deleteInstructor(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null ) return NotFound(new { message = "No instructor found with this id !"});

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"instructor with id {id} was deleted successfully "});

        }
    }
}
