using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;


namespace OTP_Helper.Helpers
{
    public static class PasswordHelper
    {
        // Hashes a plain password
        public static Task<string> HashPasswordAsync(string plainPassword)
        {
            var hashed = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            return Task.FromResult(hashed);
        }

        // Verifies a plain password against a hashed password
        public static Task<bool> VerifyPasswordAsync(string plainPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword)) return Task.FromResult(false);
            var ok = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
            return Task.FromResult(ok);
        }
    }
}
