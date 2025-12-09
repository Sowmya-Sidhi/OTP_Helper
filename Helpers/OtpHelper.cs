using System.Security.Cryptography;
using System.Text;

namespace OTP_Helper.Helper
{
    public class OtpHelper
    {
        // Generates a numeric OTP of specified digit length
        public static string GenerateNumericOtp(int digits = 6) // Default to 6 digits
        {
            // Calculate the maximum value for the given number of digits
            var max = (int)Math.Pow(10, digits);
            // Generate a random integer between 0 and max-1
            var value = RandomNumberGenerator.GetInt32(0, max);
            // Format the integer as a zero-padded string of the specified digit length
            // Default format string "D" pads with leading zeros
            return value.ToString($"D{digits}");
        }

        // Hashes the OTP with a generated salt using SHA-256
        public static (string hash, string salt) HashOtp(string otp)
        {
            // Generate a unique salt
            var salt = Guid.NewGuid().ToString("N");
            var raw = otp + "|" + salt;
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hashed = SHA256.HashData(bytes);
            return (Convert.ToBase64String(hashed), salt);
        }

        // Verifies the hashed OTP against the provided OTP and salt
        public static bool VerifyHashedOtp(string otp, string salt, string storedHash)
        {
            // Combine the OTP and salt and hash them
            var raw = otp + "|" + salt;
            var computed = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
            try
            {
                var a = Convert.FromBase64String(computed);
                var b = Convert.FromBase64String(storedHash);
                return CryptographicOperations.FixedTimeEquals(a, b);
            }
            catch
            {
                return false;
            }
        }
    }
}
