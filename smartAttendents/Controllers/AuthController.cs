using Microsoft.AspNetCore.Mvc;
using smartAttendents.Dtos;
using smartAttendents.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace smartAttendents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDto dto)
        {
            if ((dto.Role ?? "").Trim().Equals("admin", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Admin creation is disabled from this endpoint." });
            try
            {
                var res = await _auth.RegisterAsync(dto);
                return Ok(new { message = "User created", data = res });
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (InvalidOperationException e) { return Conflict(new { message = e.Message }); }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var res = await _auth.LoginAsync(dto);
                return Ok(new { message = "Logged in", data = res });
            }
            catch (InvalidOperationException) { return Unauthorized(new { message = "Invalid username or password." }); }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
            var studentId = User.FindFirst("studentId")?.Value;
            var instructorId = User.FindFirst("instructorId")?.Value;
            var username = User.Identity?.Name ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "";
            return Ok(new { role, studentId, instructorId, username });
        }
    }
}
