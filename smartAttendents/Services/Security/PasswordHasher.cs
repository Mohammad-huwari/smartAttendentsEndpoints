using System.Security.Cryptography;
using System.Text;

namespace smartAttendents.Services.Security
{
    public class PasswordHasher
    {
        public static (string hashB64, string saltB64) Create(string password)
        {
            using var hmac = new HMACSHA512();                 // salt = key
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public static bool Verify(string password, string storedHashB64, string storedSaltB64)
        {
            var salt = Convert.FromBase64String(storedSaltB64);
            using var hmac = new HMACSHA512(salt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var stored = Convert.FromBase64String(storedHashB64);
            return CryptographicOperations.FixedTimeEquals(computed, stored);
        }
    }
}
