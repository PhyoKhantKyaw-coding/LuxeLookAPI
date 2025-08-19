using System.Security.Cryptography;
using System.Text;

namespace LuxeLookAPI.Share
{
    public class CommonAuthentication
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash)
        {
            using (var sha512 = SHA512.Create())
            {
                passwordHash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Verify by comparing password hash with stored hash
        public static bool VerifyPasswordHash(string password, byte[] storedHash)
        {
            using (var sha512 = SHA512.Create())
            {
                var computedHash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}
