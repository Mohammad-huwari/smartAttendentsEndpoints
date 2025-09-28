using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using smartAttendents.Models;

namespace smartAttendents.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config) => _config = config;

        public string CreateToken(UserModel user)
        {
            var role = (user.Role ?? "student").Trim().ToLowerInvariant();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, role),
                
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            // ⬇️ هذول أهم سطرين
            if (user.StudentID.HasValue)
                claims.Add(new Claim("studentId", user.StudentID.Value.ToString()));
            if (user.InstructorID.HasValue)
                claims.Add(new Claim("instructorId", user.InstructorID.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
