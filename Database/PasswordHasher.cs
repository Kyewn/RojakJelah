using System;
using System.Security.Cryptography;

public static class PasswordHasher
{
    /// <summary>
    /// Size of salt.
    /// </summary>
    private const int SaltSize = 16;

    /// <summary>
    /// Size of hash.
    /// </summary>
    private const int HashSize = 20;

    /// <summary>
    /// Creates a hash from a password with the specified number of iterations.
    /// </summary>
    /// <remarks>
    /// The higher the iteration, the longer it takes to compute the hash (stronger security).
    /// Iteration count depends on device performance.
    /// 10k iterations is the default minimum, but 100k iterations is recommended.
    /// The hash function can be made scalable but some additional modifications are needed for the PasswordHasher.
    /// </remarks>
    /// <param name="password">The password to be hashed.</param>
    /// <param name="iterations">The number of iterations with default value of 10000.</param>
    /// <returns>Password hash.</returns>
    public static string Hash(string password, int iterations = 10000)
    {
        // Generate a random salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

        // Generate hash
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        var hash = pbkdf2.GetBytes(HashSize);

        // Combine the salt and hash
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        // Convert the combined hash to base64
        var base64Hash = Convert.ToBase64String(hashBytes);

        return base64Hash;
    }

    /// <summary>
    /// Verifies a password against a password hash stored in the database.
    /// </summary>
    /// <param name="password">Password to verify.</param>
    /// <param name="savedPasswordHash">Password hash stored in database to be checked against.</param>
    /// <returns>True if correct password, False if incorrect password.</returns>
    public static bool Verify(string password, string savedPasswordHash, int iterations = 10000)
    {
        // Extract bytes from saved password hash
        var hashBytes = Convert.FromBase64String(savedPasswordHash);

        // Get the salt from saved password hash
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        // Compute the hash on the entered password
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        var hash = pbkdf2.GetBytes(HashSize);

        // Compare hash of entered password and saved password
        for (int i = 0; i < HashSize; i++)
        {
            if (hashBytes[i+16] != hash[i])
            {
                return false;
            }
        }

        return true;
    }
}