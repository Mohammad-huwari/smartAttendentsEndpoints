using smartAttendents.Models;

namespace smartAttendents.Services.Auth
{
    public interface ITokenService
    {
        string CreateToken(UserModel user);
    }
}
