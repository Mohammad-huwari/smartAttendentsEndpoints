using Microsoft.EntityFrameworkCore;
using smartAttendents.Auth;
using smartAttendents.Dtos;
using smartAttendents.Dtos;
using smartAttendents.Models;
using smartAttendents.Services.Security;

namespace smartAttendents.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _token;

        public AuthService(AppDbContext db, ITokenService token)
        {
            _db = db;
            _token = token;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Username and Password are required.");

            var role = (dto.Role ?? "student").Trim().ToLowerInvariant();
            if (role is not ("student" or "instructor" or "admin"))
                throw new ArgumentException("Role must be 'student' or 'instructor' or 'admin'.");

            var exists = await _db.Users.AnyAsync(u => u.Username == dto.Username);
            if (exists) throw new InvalidOperationException("Username already exists.");

            var (hash, salt) = PasswordHasher.Create(dto.Password);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var user = new UserModel
                {
                    Username = dto.Username.Trim(),
                    DisplayName = dto.DisplayName,
                    Role = role,
                    PasswordHash = hash,
                    PasswordSalt = salt
                };

                if (role == "student")
                {
                    if (string.IsNullOrWhiteSpace(dto.UniversityID) && string.IsNullOrWhiteSpace(dto.Email))
                        throw new ArgumentException("UniversityID or Email is required for student.");

                    Students? s = null;
                    if (!string.IsNullOrWhiteSpace(dto.UniversityID))
                        s = await _db.Students.FirstOrDefaultAsync(x => x.UniversityID == dto.UniversityID);
                    if (s is null && !string.IsNullOrWhiteSpace(dto.Email))
                        s = await _db.Students.FirstOrDefaultAsync(x => x.Email == dto.Email);

                    if (s is null)
                    {
                        s = new Students
                        {
                            UniversityID = dto.UniversityID,
                            FullName = dto.FullName ?? dto.DisplayName ?? dto.Username,
                            Email = dto.Email,
                            FaceID = null,
                            IsActive = true
                        };
                        _db.Students.Add(s);
                        await _db.SaveChangesAsync(); // ياخذ StudentID
                    }
                    user.StudentID = s.StudentID;
                }
                else if (role == "instructor")
                {
                    if (string.IsNullOrWhiteSpace(dto.Email) && string.IsNullOrWhiteSpace(dto.FullName))
                        throw new ArgumentException("Email or FullName is required for instructor.");

                    Instructors? i = null;
                    if (!string.IsNullOrWhiteSpace(dto.Email))
                        i = await _db.Instructors.FirstOrDefaultAsync(x => x.Email == dto.Email);
                    if (i is null && !string.IsNullOrWhiteSpace(dto.FullName))
                        i = await _db.Instructors.FirstOrDefaultAsync(x => x.FullName == dto.FullName);

                    if (i is null)
                    {
                        i = new Instructors
                        {
                            FullName = dto.FullName ?? dto.DisplayName ?? dto.Username,
                            Email = dto.Email,
                            Password = null // الحساب على Users
                        };
                        _db.Instructors.Add(i);
                        await _db.SaveChangesAsync(); // ياخذ InstructorID
                    }
                    user.InstructorID = i.InstructorID;
                }
                // admin: بدون ربط

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                var token = _token.CreateToken(user); // TokenService عندك أصلاً يضيف studentId/instructorId claims
                return new AuthResponse(user.Id, user.Username, user.DisplayName, user.Role, token);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<AuthResponse> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
            if (user is null || !PasswordHasher.Verify(dto.Password, user.PasswordHash, user.PasswordSalt))
                throw new InvalidOperationException("Invalid username or password.");

            var token = _token.CreateToken(user);
            return new AuthResponse(user.Id, user.Username, user.DisplayName, user.Role, token);
        }
    }
}
