using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AppointAid.Utilities
{
    public static class PasswordHasher
    {
        // Hash a password with optional salt
        public static string HashPassword(string password, string predefinedSalt = null)
        {
            byte[] salt;

            if (predefinedSalt != null)
            {
                // Use the predefined salt if provided
                salt = Convert.FromBase64String(predefinedSalt);
            }
            else
            {
                // Generate a new random salt
                salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
            }

            // Derive a 256-bit subkey (HMACSHA256) using PBKDF2
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Combine the salt and the hashed password for storage
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        // Verify a password
        public static bool VerifyPassword(string hashedPasswordWithSalt, string passwordToCheck)
        {
            // Split the stored hash into salt and hash
            var parts = hashedPasswordWithSalt.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            string storedHash = parts[1];

            // Hash the provided password using the stored salt
            string hashedToCheck = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: passwordToCheck,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Compare the hashes
            return hashedToCheck == storedHash;
        }
    }
}