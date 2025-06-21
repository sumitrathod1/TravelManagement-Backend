using Konscious.Security.Cryptography;
using System;
using System.Text;

public class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Argon2 hashing
        using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
        {
            // Set parameters for hashing
            argon2.DegreeOfParallelism = 8; // Number of parallel threads (e.g., 8)
            argon2.MemorySize = 1024 * 1024; // Memory usage in KB (e.g., 1GB)
            argon2.Iterations = 4; // Number of iterations (e.g., 4)

            byte[] hash = argon2.GetBytes(32); // 32 bytes for the hash

            return Convert.ToBase64String(hash); // Return Base64 string for storage
        }
    }

    public static bool VerifyPassword(string password, string base64Hash)
    {
        byte[] hash = Convert.FromBase64String(base64Hash);

        // To verify, simply hash the provided password and compare
        var hashToVerify = HashPassword(password);

        return hashToVerify == Convert.ToBase64String(hash);
    }
}
