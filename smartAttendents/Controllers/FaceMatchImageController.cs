using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartAttendents.Models;
using System.Runtime.Versioning;

namespace smartAttendents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaceMatchImageController : Controller
    {
        private readonly AppDbContext _context;

        public FaceMatchImageController(AppDbContext context)
        {
            _context = context;
        }


        // get all 
        [HttpGet]
        public async Task<ActionResult<FaceMatchImage>> GetFaceMatchImages()
        {
            var faceMatchImages = await _context.FaceMatchImages.AsNoTracking().ToListAsync();
            return Ok(new
            {
                message = "data was fetched successfully",
                data = faceMatchImages
            });
        }

        // get by id
        [HttpGet("{id}")]
        public async Task<ActionResult<FaceMatchImage>> GetFaceMatchImagesById(int id)
        {
            var target = await _context.FaceMatchImages.FindAsync(id);
            if (target == null) return NotFound(new { message = $"FaceMatchImage with this id {id} not found" });

            return Ok(new { 
                message = $"FaceMatchImages with id {id} : ",
                data = target });
        }

        // create 
        [HttpPost]
        public async Task<ActionResult<FaceMatchImage>> CreateFaceMatchImages(FaceMatchImage faceMatchImage)
        {
            
            var eventExists = await _context.FaceRecognitionEvents
            .AnyAsync(e => e.EventID == faceMatchImage.EventID.Value);

            if (!eventExists)
                return BadRequest(new { message = $"FaceRecognitionEvent {faceMatchImage.EventID} does not exist." });

            _context.FaceMatchImages.Add(faceMatchImage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFaceMatchImagesById) ,
                new {id = faceMatchImage.ImageID} ,
                new {message = "created successfully " ,
                    data = faceMatchImage
                });

        }

        // update 
        [HttpPut("{id}")]
        public async Task<ActionResult<FaceMatchImage>> UpdateFaceMatchImages(int id , FaceMatchImage faceMatchImage)
        {
            if (id != faceMatchImage.ImageID) return BadRequest();
            var target = await _context.FaceMatchImages.FindAsync(id);
            if (target == null) return NotFound(new { message = "no face image found with this id " });

            var eventExists = await _context.FaceRecognitionEvents
           .AnyAsync(e => e.EventID == faceMatchImage.EventID.Value);

            if (!eventExists)
                return BadRequest(new { message = $"FaceRecognitionEvent {faceMatchImage.EventID} does not exist." });

            _context.Entry(target).CurrentValues.SetValues(faceMatchImage);
            await _context.SaveChangesAsync();

            return Ok(new {
                message = "updated successfully :" ,
                data = faceMatchImage
            });

        }

        // delete 
        [HttpDelete("{id}")]
        public async Task<ActionResult<FaceMatchImage>> DeleteFaceMatchImages(int id)
        {
            var target = await _context.FaceMatchImages.FindAsync(id);
            if (target == null) return NotFound(new {message = $"FaceMatchImage with id {id} not found " });

            _context.FaceMatchImages.Remove(target);
             await _context.SaveChangesAsync();

            return Ok(new { message = $"FaceMatchImage with id {id} was deleted successfully" });
        }


    }
}
